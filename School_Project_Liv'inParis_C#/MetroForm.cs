using System;
using Visual_Studio;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Microsoft.VisualBasic.Devices;
using System.Drawing;
using System.Windows.Forms;

namespace Visual_Studio
{
    /// <summary>
    /// Classe représentant une carte interactive du métro de Paris
    /// </summary>
    public class MetroForm : Form
    {
        // Dictionnaire des stations avec leur ID comme clé
        private Dictionary<int, Station> stations = new Dictionary<int, Station>();

        // Store final connections. Key is Tuple<fromId, toId, lineId>
        private Dictionary<Tuple<int, int, string>, Lien> connections = new Dictionary<Tuple<int, int, string>, Lien>();

        // Ensemble des IDs des stations terminales
        private HashSet<int> terminalStationIds = new HashSet<int>();

        // Coordinate scaling variables (same)
        private float minLon, maxLon, minLat, maxLat;
        private bool dataLoadedSuccessfully = false;
        private float margin = 50;

        // --- Stockage du chemin PCM ---
        private List<int> parcoursPCM;
        private HashSet<Tuple<int, int>> pcmSegments; // Pour vérifier si un segment fait partie du chemin

        // Drawing resources (same)
        private Dictionary<string, Pen> linePens = new Dictionary<string, Pen>();
        // private Dictionary<string, Brush> lineBrushes; // Plus nécessaire si on n'utilise pas les flèches manuelles
        private Pen pcmPathPen; // Stylo dédié pour le chemin PCM
        private Brush stationBrush = Brushes.White;
        private Pen stationOutlinePen = new Pen(Color.Black, 1.5f);
        private Font terminalStationFont = new Font("Segoe UI", 7f, FontStyle.Regular);
        private Brush textBrush = Brushes.WhiteSmoke;
        private float stationRadius = 4f;
        private AdjustableArrowCap arrowCap;

        // Color mapping (same)
        private Dictionary<string, Color> lineColors = new Dictionary<string, Color>
        {
            { "1", Color.FromArgb(255, 205, 0) },    // Jaune
            { "2", Color.FromArgb(0, 97, 175) },    // Bleu
            { "3", Color.FromArgb(140, 148, 0) },    // Olive
            { "3bis", Color.FromArgb(108, 205, 225) }, // Cyan
            { "4", Color.FromArgb(187, 51, 148) },   // Magenta
            { "5", Color.FromArgb(243, 146, 49) },   // Orange
            { "6", Color.FromArgb(118, 198, 172) },  // Vert
            { "7", Color.FromArgb(240, 148, 185) },  // Rose
            { "7bis", Color.FromArgb(118, 198, 172) }, // Vert
            { "8", Color.FromArgb(196, 160, 210) },  // Lilas
            { "9", Color.FromArgb(206, 210, 0) },    // Vert
            { "10", Color.FromArgb(215, 174, 0) },   // Jaune Foncé
            { "11", Color.FromArgb(124, 81, 35) },   // Marron
            { "12", Color.FromArgb(0, 128, 75) },    // Vert Foncé
            { "13", Color.FromArgb(111, 194, 224) }, // Bleu
            { "14", Color.FromArgb(98, 36, 128) },   // Violet Foncé
            { "DEFAULT", Color.FromArgb(255, 0, 0) } // Rouge
        };
        //couleur sommet pour la coloration des sommets
        private Dictionary<int, Color> pointColors = new Dictionary<int, Color>();
        private static List<Color> couleurPalette = new List<Color>
        {
        Color.Red,
        Color.Blue,
        Color.Green,
        Color.Orange,
        Color.Purple,
        Color.Yellow
        };


        /// <summary>
        /// Constructeur de la carte du métro
        /// </summary>
        /// <param name="PCM">Liste des IDs des stations du plus court chemin</param>
        public MetroForm(List<int> PCM, Dictionary<int, int> ColorNoeud)
        {
            this.parcoursPCM = PCM ?? new List<int>();
            this.pcmSegments = new HashSet<Tuple<int, int>>();
            if (this.parcoursPCM.Count > 1)
            {
                for (int i = 0; i < this.parcoursPCM.Count - 1; i++)
                {
                    int u = this.parcoursPCM[i]; int v = this.parcoursPCM[i + 1];
                    pcmSegments.Add(Tuple.Create(u, v));
                    pcmSegments.Add(Tuple.Create(v, u)); // Ajouter les deux sens pour la recherche
                }
            }
            //Console.WriteLine($"MetroForm received PCM with {this.parcoursPCM.Count} stations.");

            //colorie le graphe avec des points de couleur.
            foreach (KeyValuePair<int, int> somColor in ColorNoeud)
            {
                int nbColorAssocié = somColor.Value;
                this.pointColors[somColor.Key] = couleurPalette[nbColorAssocié];
            }

            // Configuration de la fenêtre
            this.Text = "Carte du Métro de Paris";
            this.Size = new Size(1024, 768);
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(600, 400);
            this.CenterToScreen();
            this.DoubleBuffered = true;

            InitializeDrawingResources();


            // Chargement des données des stations et connexions
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string nodesFilePath = Path.Combine(baseDirectory, "Noeuds.csv");
            string arcsFilePath = Path.Combine(baseDirectory, "Arcs.csv");
            Encoding fileEncoding = Encoding.UTF8;
            //Console.WriteLine($"Attempting to use Encoding: {fileEncoding.EncodingName}");

            LoadStations(nodesFilePath, fileEncoding);
            if (dataLoadedSuccessfully)
            {
                LoadConnections(arcsFilePath, fileEncoding);
                CalculateScreenCoordinates();
            }

            this.Paint += new PaintEventHandler(MetroForm_Paint);
            this.Resize += (sender, args) => { if (dataLoadedSuccessfully) CalculateScreenCoordinates(); this.Invalidate(); };


        }

        /// <summary>
        /// Initialise les ressources graphiques (stylos, couleurs, etc.)
        /// </summary>
        private void InitializeDrawingResources()
        {
            linePens.Clear();
            // lineBrushes.Clear(); // Plus nécessaire

            foreach (var kvp in lineColors)
            {
                // Utiliser une épaisseur standard pour les lignes normales
                var pen = new Pen(kvp.Value, 2.0f) { LineJoin = LineJoin.Round };
                linePens.Add(kvp.Key, pen);
                // lineBrushes.Add(kvp.Key, new SolidBrush(kvp.Value)); // Plus nécessaire
            }
            if (!linePens.ContainsKey("DEFAULT")) linePens.Add("DEFAULT", new Pen(lineColors["DEFAULT"], 2.0f) { LineJoin = LineJoin.Round });
            // if (!lineBrushes.ContainsKey("DEFAULT")) lineBrushes.Add("DEFAULT", new SolidBrush(lineColors["DEFAULT"])); // Plus nécessaire

            // Créer le stylo pour le chemin PCM
            pcmPathPen = new Pen(Color.Red, 3.0f) // Légèrement plus épais et rouge
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // *** RE-INITIALISER arrowCap ***
            arrowCap = new AdjustableArrowCap(5, 5, true);
        }



        /// <summary>
        /// Charge les stations depuis un fichier CSV
        /// </summary>
        /// <param name="filePath">Chemin du fichier Noeuds.csv</param>
        /// <param name="encoding">Encodage du fichier</param>
        private void LoadStations(string filePath, Encoding encoding)
        {
            stations.Clear();
            dataLoadedSuccessfully = false;
            bool firstStation = true;
            int lineNum = 0; // For error reporting

            try
            {
                var lines = File.ReadLines(filePath, encoding).Skip(1); // Skip header

                foreach (string line in lines)
                {
                    lineNum++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(';');
                    // --- Use CORRECT Indices based on user description ---
                    // 0: ID, 1: Line, 2: Name, 3: Lon, 4: Lat
                    if (parts.Length >= 5)
                    {
                        string idStr = parts[0].Trim();
                        string lineName = parts[1].Trim(); // Primary line
                        string stationName = parts[2].Trim();
                        string lonStr = parts[3].Trim().Replace(',', '.');
                        string latStr = parts[4].Trim().Replace(',', '.');

                        if (int.TryParse(idStr, out int id) && !string.IsNullOrWhiteSpace(stationName) && // Ensure name is not empty
                            float.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out float lon) &&
                            float.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out float lat))
                        {
                            var station = new Station(id, stationName, lineName, lon, lat);
                            if (!stations.ContainsKey(id))
                            {
                                stations.Add(id, station);
                                if (firstStation) { minLon = maxLon = lon; minLat = maxLat = lat; firstStation = false; }
                                else { minLon = Math.Min(minLon, lon); maxLon = Math.Max(maxLon, lon); minLat = Math.Min(minLat, lat); maxLat = Math.Max(maxLat, lat); }
                            }
                        }
                        else { Console.WriteLine($"Format Error (Node Line {lineNum}): Could not parse data: ID='{idStr}', Name='{stationName}', Lon='{lonStr}', Lat='{latStr}'"); }
                    }
                    else { Console.WriteLine($"Format Error (Node Line {lineNum}): Incorrect columns ({parts.Length}): {line}"); }
                }

                if (stations.Count > 0)
                {
                    //Console.WriteLine($"Chargé {stations.Count} stations distinctes."); 
                    //Console.WriteLine($"Lon Range: {minLon:F7} to {maxLon:F7}"); 
                    //Console.WriteLine($"Lat Range: {minLat:F7} to {maxLat:F7}"); 
                    dataLoadedSuccessfully = true;
                }
                else { MessageBox.Show("Aucune station n'a été chargée. Vérifiez Noeuds.csv.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            catch (FileNotFoundException) { MessageBox.Show($"Fichier non trouvé: {filePath}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); dataLoadedSuccessfully = false; }
            catch (IOException e) { MessageBox.Show($"Erreur Fichier E/S: {filePath}\n{e.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); dataLoadedSuccessfully = false; }
            catch (Exception e) { MessageBox.Show($"Erreur inattendue (Stations Ligne {lineNum}): {e.Message}\n{e.StackTrace}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); dataLoadedSuccessfully = false; }
        }



        /// <summary>
        /// Charge les connexions entre stations depuis un fichier CSV
        /// </summary>
        /// <param name="arcsfilePath">Chemin du fichier Arcs.csv</param>
        /// <param name="encoding">Encodage du fichier</param>
        private void LoadConnections(string arcsfilePath, Encoding encoding)
        {
            connections.Clear(); // Final connections dictionary
            terminalStationIds.Clear();

            // Dictionaries to store relationships derived from Arcs.csv
            // Key: CurrentStationID
            // Value: (LinkedStationID, LineID_of_CurrentStation)
            var prevMap = new Dictionary<int, (int prevId, string lineId)>();
            var nextMap = new Dictionary<int, (int nextId, string lineId)>();

            // Track which stations explicitly have blank prev/next entries
            var hasBlankPrev = new HashSet<int>();
            var hasBlankNext = new HashSet<int>();

            int lineNum = 1;

            try
            {
                // --- Phase 1: Populate Prev/Next Maps ---
                var lines = File.ReadLines(arcsfilePath, encoding).Skip(1);
                foreach (string line in lines)
                {
                    lineNum++;
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(';');

                    // Indices: 0=Current_ID, 2=Prev_ID, 3=Next_ID
                    if (parts.Length >= 4) // Need at least Current, Prev, Next
                    {
                        string currentIdStr = parts[0].Trim();
                        string prevIdStr = parts[2].Trim();
                        string nextIdStr = parts[3].Trim();

                        if (!int.TryParse(currentIdStr, out int currentStationId) || !stations.ContainsKey(currentStationId))
                        {
                            Console.WriteLine($"Warning (Arc Line {lineNum}): Invalid/unknown Current Station ID '{currentIdStr}'. Skipping row.");
                            continue;
                        }

                        // Get Line ID from the current station's primary line data
                        string lineId = stations[currentStationId].PrimaryLine.Trim(); // Use the line from Noeuds.csv

                        // Process Previous ID
                        if (int.TryParse(prevIdStr, out int prevStationId) && prevStationId != 0)
                        {
                            if (stations.ContainsKey(prevStationId)) // Only map if Prev station exists
                            {
                                prevMap[currentStationId] = (prevStationId, lineId);
                            }
                            else { Console.WriteLine($"Warning (Arc Line {lineNum}): Prev station ID {prevStationId} (from current {currentStationId}) not found in stations list."); }
                        }
                        else if (string.IsNullOrWhiteSpace(prevIdStr))
                        {
                            hasBlankPrev.Add(currentStationId); // Mark as having a blank previous
                        }

                        // Process Next ID
                        if (int.TryParse(nextIdStr, out int nextStationId) && nextStationId != 0)
                        {
                            if (stations.ContainsKey(nextStationId)) // Only map if Next station exists
                            {
                                nextMap[currentStationId] = (nextStationId, lineId);
                            }
                            else { Console.WriteLine($"Warning (Arc Line {lineNum}): Next station ID {nextStationId} (from current {currentStationId}) not found in stations list."); }
                        }
                        else if (string.IsNullOrWhiteSpace(nextIdStr))
                        {
                            hasBlankNext.Add(currentStationId); // Mark as having a blank next
                        }
                    }
                    else { Console.WriteLine($"Format Error (Arc Line {lineNum}): Insufficient columns ({parts.Length}): {line}"); }
                }

                //Console.WriteLine($"Populated Prev map ({prevMap.Count} entries) and Next map ({nextMap.Count} entries).");

                // --- Phase 2: Build Connections and Check Bidirectionality ---
                int bidirectionalPairsFound = 0;
                // Iterate through the Next map to find A->B segments
                foreach (var kvpA in nextMap)
                {
                    int stationA_Id = kvpA.Key;
                    int stationB_Id = kvpA.Value.nextId;
                    string lineL_A = kvpA.Value.lineId; // Line defined by station A

                    // Get station objects
                    if (stations.TryGetValue(stationA_Id, out Station stationA) &&
                        stations.TryGetValue(stationB_Id, out Station stationB))
                    {
                        var connection = new Lien(stationA, stationB, lineL_A);

                        // Check if B -> A exists using the prevMap
                        // Does B have an entry in prevMap?
                        if (prevMap.TryGetValue(stationB_Id, out var prevDataB))
                        {
                            int prevStationOfB = prevDataB.prevId;
                            string lineL_B = prevDataB.lineId; // Line defined by station B

                            // Is the previous station of B actually A? AND are the lines the same?
                            if (prevStationOfB == stationA_Id && lineL_A == lineL_B)
                            {
                                connection.IsBidirectional = true;
                                // Count unique pairs once
                                if (stationA_Id < stationB_Id) bidirectionalPairsFound++;
                            }
                            // If lines differ, treat as separate unidirectional segments if needed,
                            // but current structure assumes connection uses one line ID.
                            else if (prevStationOfB == stationA_Id && lineL_A != lineL_B)
                            {
                                Console.WriteLine($"Warning: Bidirectional check failed for {stationA_Id}-{stationB_Id}. Found reverse link ({prevStationOfB}<-{stationB_Id}) but lines differ ('{lineL_A}' vs '{lineL_B}'). Treating {stationA_Id}->{stationB_Id} as unidirectional for line '{lineL_A}'.");
                            }
                        }
                        // else: No B->A found based on B's previous entry, so A->B is unidirectional.

                        // Add the connection representing the A->B direction.
                        connections.TryAdd(connection.GetKey(), connection);
                    }
                }


                // Identify Terminals - Stations marked with blank prev OR blank next
                foreach (int stationId in hasBlankPrev) terminalStationIds.Add(stationId);
                foreach (int stationId in hasBlankNext) terminalStationIds.Add(stationId);


                //Console.WriteLine($"Créé {connections.Count} connexions finales (représentant chaque direction trouvée via Next map).");
                //Console.WriteLine($"Trouvé {bidirectionalPairsFound} paires physiques bidirectionnelles (basé sur Prev/Next concordance).");
                //Console.WriteLine($"Identifié {terminalStationIds.Count} stations terminales (basé sur Préc/Suiv vide dans Arcs.csv).");

            }
            catch (FileNotFoundException) { MessageBox.Show($"Fichier connexions non trouvé: {arcsfilePath}", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (IOException e) { MessageBox.Show($"Erreur Fichier E/S: {arcsfilePath}\n{e.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception e) { MessageBox.Show($"Erreur inattendue (Connexions Ligne {lineNum}): {e.Message}\n{e.StackTrace}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }



        /// <summary>
        /// Calcule les coordonnées à l'écran à partir des coordonnées géographiques
        /// </summary>
        private void CalculateScreenCoordinates()
        {
            // --- No changes needed from previous version ---
            if (!dataLoadedSuccessfully || stations.Count == 0 || this.ClientSize.Width <= 2 * margin || this.ClientSize.Height <= 2 * margin) { Console.WriteLine("Skipping coordinate calculation."); return; }
            float drawWidth = this.ClientSize.Width - 2 * margin; float drawHeight = this.ClientSize.Height - 2 * margin;
            float deltaLon = (maxLon - minLon); float deltaLat = (maxLat - minLat);
            float scaleX = (Math.Abs(deltaLon) < 1e-9) ? 1 : drawWidth / deltaLon; float scaleY = (Math.Abs(deltaLat) < 1e-9) ? 1 : drawHeight / deltaLat;
            // Optional: Preserve aspect ratio // scaleX = scaleY = Math.Min(scaleX, scaleY);
            float offsetX = margin + (drawWidth - deltaLon * scaleX) / 2f; float offsetY = margin + (drawHeight - deltaLat * scaleY) / 2f;
            //Console.WriteLine($"Calculating Coords: ScaleX={scaleX}, ScaleY={scaleY}, OffsetX={offsetX}, OffsetY={offsetY}");
            foreach (var station in stations.Values)
            {
                float x = (station.GeoCoordinates.X - minLon) * scaleX + offsetX;
                float y = drawHeight - (station.GeoCoordinates.Y - minLat) * scaleY + offsetY;
                if (float.IsNaN(x) || float.IsInfinity(x) || float.IsNaN(y) || float.IsInfinity(y))
                {
                    Console.WriteLine($"ERROR: Invalid coordinates for station {station.Id}. Using center fallback.");
                    x = this.ClientSize.Width / 2f; y = this.ClientSize.Height / 2f;
                }
                station.ScreenCoordinates = new PointF(x, y);
            }
            //Console.WriteLine("Coordinate calculation complete.");
        }



        /// <summary>
        /// Gère l'événement de peinture de la fenêtre
        /// </summary>
        private void MetroForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Affichage d'un message d'erreur si le chargement a échoué
            if (!dataLoadedSuccessfully || stations.Count == 0)
            {
                using (Font errorFont = new Font("Segoe UI", 12))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Brush errorBrush = new SolidBrush(Color.FromArgb(255, 100, 100))) // Rouge foncé sur Noir
                { g.DrawString(dataLoadedSuccessfully ? "Aucune station." : "Erreur chargement données.", errorFont, errorBrush, this.ClientRectangle, sf); }
                return;
            }
            if (this.ClientSize.Width <= 2 * margin || this.ClientSize.Height <= 2 * margin) return;

            // --- 1. Draw Connections ---
            foreach (var connection in connections.Values)
            {
                try
                {
                    //PointF p1 = connection.FromStation.ScreenCoordinates;
                    //PointF p2 = connection.ToStation.ScreenCoordinates;

                    // Debug : Affichage des coordonnées
                    //Console.WriteLine($"Drawing line: {connection.FromStation.Name} -> {connection.ToStation.Name}");
                    //Console.WriteLine($"From: ({p1.X}, {p1.Y}) To: ({p2.X}, {p2.Y})");

                    // Vérification des coordonnées valides
                    //if (float.IsNaN(p1.X) || float.IsNaN(p1.Y) || float.IsNaN(p2.X) || float.IsNaN(p2.Y) ||
                    //    float.IsInfinity(p1.X) || float.IsInfinity(p1.Y) || float.IsInfinity(p2.X) || float.IsInfinity(p2.Y))
                    //{
                    //    Console.WriteLine($"Skipping invalid line: {connection.FromStation.Name} -> {connection.ToStation.Name}");
                    //    continue;
                    //}

                    // --- Choisir le stylo ---
                    var segmentKey = Tuple.Create(connection.FromStation.Id, connection.ToStation.Id);
                    bool isPcmSegment = pcmSegments.Contains(segmentKey);

                    /*
                    foreach (var segment in pcmSegments)
                    {
                        Console.WriteLine($"Segment PCM: {segment.Item1} -> {segment.Item2}");
                    }
                    */

                    // Utilise le bon stylo pris grâce au numéro de ligne
                    Pen penToUse = linePens.ContainsKey(connection.Line) ? linePens[connection.Line] : null;

                    // Si ce segment NE FAIT PAS partie du PCM, on le dessine maintenant
                    if (!isPcmSegment)
                    {
                        try
                        {
                            PointF p1 = connection.FromStation.ScreenCoordinates;
                            PointF p2 = connection.ToStation.ScreenCoordinates;

                            if (float.IsNaN(p1.X) || float.IsNaN(p1.Y) || float.IsNaN(p2.X) || float.IsNaN(p2.Y) ||
                                float.IsInfinity(p1.X) || float.IsInfinity(p1.Y) || float.IsInfinity(p2.X) || float.IsInfinity(p2.Y))
                            {
                                continue; // Ignore connexion invalide
                            }

                            // Utilise le stylo de la couleur de ligne normale
                            string cleanLineId = new string(connection.Line.TakeWhile(char.IsDigit).ToArray());
                            Pen basePen = linePens.TryGetValue(cleanLineId, out Pen foundPen) ? foundPen : linePens["DEFAULT"];

                            // Important: Utilise un clone pour modifications temporaires (flèche)
                            using (Pen currentPen = (Pen)basePen.Clone())
                            {
                                currentPen.Width = 2.0f; // Épaisseur standard

                                if (!connection.IsBidirectional)
                                {
                                    currentPen.CustomEndCap = arrowCap;
                                    g.DrawLine(currentPen, p1, p2);
                                }
                                else
                                {
                                    // Pour bidirectionnelles, dessine une seule fois
                                    if (connection.FromStation.Id < connection.ToStation.Id)
                                    {
                                        g.DrawLine(currentPen, p1, p2);
                                    }
                                }
                            } // Fin using currentPen (clone)
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur dessin connexion normale {connection?.FromStation?.Id}-{connection?.ToStation?.Id}: {ex.Message}");
                        }
                    } // Fin if (!isPcmSegment)
                }
                catch (Exception Ex) { Console.WriteLine("Erreur dans le passage 1"); }

                // --- PASSE 2 : Dessin du CHEMIN PCM (par-dessus les autres lignes) ---
                if (pcmSegments != null) // Seulement s'il y a un chemin à dessiner
                {
                    // On peut itérer sur les segments du chemin si on les a stockés
                    // ou re-itérer sur les connexions et ne dessiner que le PCM.
                    // Re-itérer est plus simple ici :
                    foreach (var lien in connections.Values)
                    {
                        var segmentKey1 = Tuple.Create(lien.FromStation.Id, lien.ToStation.Id);
                        var segmentKey2 = Tuple.Create(lien.ToStation.Id, lien.FromStation.Id);
                        bool isPcmSegment = pcmSegments.Contains(segmentKey1) || pcmSegments.Contains(segmentKey2);

                        if (isPcmSegment)
                        {
                            try
                            {
                                PointF p1 = lien.FromStation.ScreenCoordinates;
                                PointF p2 = lien.ToStation.ScreenCoordinates;

                                if (float.IsNaN(p1.X) || float.IsNaN(p1.Y) || float.IsNaN(p2.X) || float.IsNaN(p2.Y) ||
                                    float.IsInfinity(p1.X) || float.IsInfinity(p1.Y) || float.IsInfinity(p2.X) || float.IsInfinity(p2.Y))
                                {
                                    continue;
                                }

                                // Utilise le stylo PCM dédié (rouge et épais)
                                // Dessine seulement une fois par segment physique pour éviter surépaisseur inutile
                                if (lien.FromStation.Id < lien.ToStation.Id)
                                {
                                    g.DrawLine(pcmPathPen, p1, p2);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erreur dessin segment PCM {lien?.FromStation?.Id}-{lien?.ToStation?.Id}: {ex.Message}");
                            }
                        } // Fin if (isPcmSegment)
                    } // Fin Deuxième Passe (chemin PCM)
                }

                // --- PASSE 3 : Dessin des STATIONS (par-dessus toutes les lignes) ---
                // Utilise un pinceau local pour le texte des ID/Noms
                using (SolidBrush textIdBrush = new SolidBrush(Color.WhiteSmoke))
                {
                    foreach (var station in stations.Values)
                    {
                        try
                        {
                            PointF screenPos = station.ScreenCoordinates;
                            if (float.IsNaN(screenPos.X) || float.IsInfinity(screenPos.X) || float.IsNaN(screenPos.Y) || float.IsInfinity(screenPos.Y))
                            {
                                continue;
                            }

                            // Détermine le pinceau STATIQUE pour le remplissage
                            Brush brushPourRemplissage = Brushes.White; 

                            // Priorité 1: Chemin PCM
                            
                            if (parcoursPCM != null && parcoursPCM.Contains(station.Id))
                            {
                                brushPourRemplissage = Brushes.HotPink; // Ou autre couleur vive
                            }
                            
                            // Priorité 2: Coloriage assigné
                            else if (this.pointColors != null && this.pointColors.TryGetValue(station.Id, out Color assignedColor))
                            {
                                // Mappe la couleur au Brush statique
                                if (assignedColor == Color.Red) brushPourRemplissage = Brushes.Red;
                                else if (assignedColor == Color.Blue) brushPourRemplissage = Brushes.Blue;
                                else if (assignedColor == Color.Green) brushPourRemplissage = Brushes.LimeGreen;
                                else if (assignedColor == Color.Orange) brushPourRemplissage = Brushes.Orange;
                                else if (assignedColor == Color.Purple) brushPourRemplissage = Brushes.Purple;
                                else if (assignedColor == Color.Yellow) brushPourRemplissage = Brushes.Yellow;
                                else if (assignedColor == Color.Cyan) brushPourRemplissage = Brushes.Cyan;
                                else if (assignedColor == Color.Magenta) brushPourRemplissage = Brushes.Magenta;
                                else if (assignedColor == Color.Brown) brushPourRemplissage = Brushes.Brown;
                                // else reste White
                            }

                            // Dessine le cercle de la station
                            g.FillEllipse(brushPourRemplissage, screenPos.X - stationRadius, screenPos.Y - stationRadius, 2 * stationRadius, 2 * stationRadius);
                            // Dessine le contour
                            g.DrawEllipse(stationOutlinePen, screenPos.X - stationRadius, screenPos.Y - stationRadius, 2 * stationRadius, 2 * stationRadius);

                            // Affiche l'ID si la station fait partie du chemin
                            if (parcoursPCM != null && parcoursPCM.Contains(station.Id))
                            {
                                string stationNom = station.Name.ToString();
                                g.DrawString(stationNom, terminalStationFont, textIdBrush, screenPos.X + stationRadius + 3, screenPos.Y - (terminalStationFont.Height / 2f));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERREUR lors du dessin de la station {station?.Id} ({station?.Name ?? "INCONNU"}): {ex.Message}");
                        }
                    }
                }
            }
        } // Fin MetroForm_Paint
                /*
                if (!isPcmSegment)
                {
                    penToUse = linePens["DEFAULT"];
                    penToUse.Width = 3.0f; // Décommenter si vous voulez le chemin gris plus épais
                    lien.IsBidirectional = true;
                }
                else
                {
                    // Utiliser le stylo de la couleur de ligne normale
                    string cleanLineId = new string(connection.Line.TakeWhile(char.IsDigit).ToArray());
                    if (string.IsNullOrEmpty(cleanLineId) || !linePens.TryGetValue(cleanLineId, out penToUse))
                    {
                        penToUse = linePens["DEFAULT"]; // Fallback normal
                    }
                    penToUse.Width = 2.0f;
                }

                if (penToUse == null)
                {
                    Console.WriteLine($"Warning: No pen found for line {connection.Line}, using default color.");
                    penToUse = linePens["DEFAULT"]; // Utiliser la couleur par défaut si la ligne n'existe pas
                }

                if (!connection.IsBidirectional)
                {
                    penToUse.CustomEndCap = arrowCap;
                    g.DrawLine(penToUse, p1, p2);
                    penToUse.CustomEndCap = null;
                }
                else
                {
                    if (connection.FromStation.Id < connection.ToStation.Id)
                    {
                        g.DrawLine(penToUse, p1, p2);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ce try catch est présent pour véiter d'avoir des erreurs lorsque l'on trace des liens orientés.
                // Je n'ai pas réussi à corriger cette erreur autrement qu'en mettant un try catch.
            }
        }
        foreach (var station in stations.Values)
        {
            try
            {
                PointF screenPos = station.ScreenCoordinates;
                if (float.IsNaN(screenPos.X) || float.IsInfinity(screenPos.X) || float.IsNaN(screenPos.Y) || float.IsInfinity(screenPos.Y)) { continue; }

                if (parcoursPCM.Contains(station.Id))
                {
                    stationBrush = Brushes.Red;
                }
                else if (this.pointColors.ContainsKey(station.Id))
                {
                    Color colr = this.pointColors[station.Id];
                    if (colr == Color.Red)
                    {
                        stationBrush = Brushes.Magenta;
                    }
                    else if (colr == Color.Blue)
                    {
                        stationBrush = Brushes.Blue;
                    }
                    else if (colr == Color.Green)
                    {
                        stationBrush = Brushes.Green;
                    }
                    else if (colr == Color.Yellow)
                    {
                        stationBrush = Brushes.Yellow;
                    }
                    else if (colr == Color.Orange)
                    {
                        stationBrush = Brushes.Orange;
                    }
                    else if (colr == Color.Purple)
                    {
                        stationBrush = Brushes.Purple;
                    }
                }
                else
                {
                    stationBrush = Brushes.White;
                }
                g.FillEllipse(stationBrush, screenPos.X - stationRadius, screenPos.Y - stationRadius, 2 * stationRadius, 2 * stationRadius);
                g.DrawEllipse(stationOutlinePen, screenPos.X - stationRadius, screenPos.Y - stationRadius, 2 * stationRadius, 2 * stationRadius);

                if (parcoursPCM.Contains(station.Id))
                {
                    string Id = Convert.ToString(station.Id);
                    g.DrawString(Id, terminalStationFont, textBrush, screenPos.X + stationRadius + 2, screenPos.Y - (terminalStationFont.Height / 2f));
                }
                /*
                if (parcoursPCM.Contains(station.Id))
                {
                    g.DrawString(station.Name, terminalStationFont, textBrush, screenPos.X + stationRadius + 2, screenPos.Y - (terminalStationFont.Height / 2f));
                }
                */
                /*
            }
            catch (Exception ex) { Console.WriteLine($"ERROR during Draw Station for {station?.Name ?? "UNKNOWN"}: {ex.Message}"); }
        }
    }
    */
                /*

                /// <summary>
                /// Libère les ressources graphiques
                /// </summary>
                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        stationBrush.Dispose();
                        stationOutlinePen.Dispose();
                        terminalStationFont.Dispose();
                        textBrush.Dispose();
                        arrowCap.Dispose();
                        foreach (var pen in linePens.Values) { pen.Dispose(); }
                        linePens.Clear();
                        // Brushes were removed from previous version, if added back, dispose here
                    }
                    base.Dispose(disposing);
                }

                */
    }
}
        