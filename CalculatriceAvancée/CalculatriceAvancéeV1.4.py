# -*- coding: utf-8 -*-
# Calculatrice V1.4

import tkinter as tk
from tkinter import messagebox



# =========================
# Constantes
# =========================



BOUTONS = [
    '7', '8', '9', '/',
    '4', '5', '6', '*',
    '1', '2', '3', '-',
    '0', '.', '=', '+'
]



# =========================
# Fonctions de calcul
# =========================



# Entrée et sortie
def cliquer(entree, valeur):
    entree.insert(tk.END, valeur)
def effacer(entree):
    entree.delete(0, tk.END)
def confirmer_fermeture(fenetre, event=None):
    reponse = messagebox.askyesno(
        'Fermeture',
        'Êtes-vous sûr(e) de vouloir fermer la calculatrice ?'
    )

    if reponse:
        fenetre.destroy()



def add(a,b):
    return float(a)+float(b)
def min(a,b):
    return float(a)-float(b)
def mult(a,b):
    return float(a)*float(b)
def div(a,b):
    return float(a)/float(b)



def calculer(entree):
    try:
        demande = entree.get()

        if "+" in demande:
            a, b = demande.split("+")
            resultat = add(a, b)

        elif "-" in demande:
            a, b = demande.split("-")
            resultat = min(a, b)

        elif "*" in demande:
            a, b = demande.split("*")
            resultat = mult(a, b)

        elif "/" in demande:
            a, b = demande.split("/")
            resultat = div(a, b)

        else:
            raise ValueError("Opérateur absent")
        entree.delete(0, tk.END)
        entree.insert(0, str(resultat))
    except Exception:
        entree.delete(0, tk.END)
        entree.insert(0, "Erreur")



# =========================
# Création de l'interface
# =========================



def creer_interface(fenetre):
    fenetre.title("Calculatrice")
    fenetre.geometry("300x400")
    fenetre.resizable(True, True)

    entree = tk.Entry(fenetre,font=("Arial", 24),bd=10,relief=tk.RIDGE,justify="right")
    entree.pack(fill="x", padx=10, pady=10, ipady=10)

    return entree

def creer_boutons(fenetre, entree):
    cadre = tk.Frame(fenetre)
    cadre.pack(expand=True, fill="both")

    ligne = 0
    colonne = 0

    for bouton in BOUTONS:
        if bouton == '=':
            cmd = lambda: calculer(entree)
        else:
            cmd = lambda t=bouton: cliquer(entree, t)
        bouton = tk.Button(cadre, text=bouton, font=("Arial", 18), command=cmd)
        bouton.grid(row=ligne,column=colonne,sticky="nsew",padx=2,pady=2)
        colonne += 1
        if colonne > 3:
            colonne = 0
            ligne += 1

    bouton_c = tk.Button(fenetre,text="C",font=("Arial", 18),command=lambda: effacer(entree))
    bouton_c.pack(fill="both", padx=10, pady=5)
    for i in range(3):
        cadre.grid_columnconfigure(i, weight=1)
        cadre.grid_rowconfigure(i, weight=1)



# =========================
# Programme principal
# =========================

def main():
    fenetre = tk.Tk()
    fenetre.bind('<Escape>', lambda event: confirmer_fermeture(fenetre))
    fenetre.protocol("WM_DELETE_WINDOW",lambda: confirmer_fermeture(fenetre))

    entree = creer_interface(fenetre)
    creer_boutons(fenetre, entree)
    fenetre.mainloop()

# Point d'entrée du programme
if __name__ == "__main__":
    main()