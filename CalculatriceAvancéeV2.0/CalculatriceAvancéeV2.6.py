# -*- coding: utf-8 -*-
#CALCULATRICE V2.6



#-------------
# IMPORTATION
#-------------

import tkinter as tk
from tkinter import messagebox
from sympy.parsing.sympy_parser import (parse_expr,standard_transformations,implicit_multiplication_application,convert_xor)
from sympy import sin, cos, tan, pi

from sympy import log # here log reprensents the neperiem logarithm
def logb(x, b): return log(x) / log(b)

from sympy import exp

import numpy as np 
import matplotlib.pyplot as plt 
from sympy import symbols, lambdify
x = symbols('x')

from sympy import expand, factor

from sympy import Eq, solve
from sympy import solve_univariate_inequality

from sympy import diff

from sympy import integrate

from sympy import limit, oo



#----------
# METHODES
#----------



transformations = standard_transformations + (
    implicit_multiplication_application,
    convert_xor,
)



# FERMETURE DE L'APPLICATION
def confirmer_fermeture(event=None):
    reponse = messagebox.askyesno(
        'Fermeture',
        'Êtes-vous sûr(e) de vouloir fermer la calculatrice ?'
    )

    if reponse:
        fenetre.destroy()



# LECTURE DU PROMPT AVEC VERIFICATION DE FONCTIONS X/SIN/COS/LOG/...
def lire_expression(texte):
    texte = preparer_expression(texte)
    return parse_expr(texte,transformations=transformations,local_dict=
        {
            'x': x,
            'sin': sin,
            'cos': cos,
            'tan': tan,
            'log': log,
            'logb': logb,
            'exp': exp,
            'pi': pi
        }
    )



# LIMITE
def limite_zero():
    try:
        expr = lire_expression(entree.get())
        L = limit(expr, x, 0)
        label_resultat.config(text=f'Limite x --> 0 : {L}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

def limite_moins_inf():
    try:
        expr = lire_expression(entree.get())
        L = limit(expr, x, -oo)
        label_resultat.config(text=f'Limite x --> -∞ : {L}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

def limite_inf():
    try:
        expr = lire_expression(entree.get())
        L = limit(expr, x, oo)
        label_resultat.config(text=f'Limite x --> +∞ : {L}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

def limite_valeur():
    try:
        texte = entree.get()
        if '->' not in texte:
            label_resultat.config(text='Format : expression -> valeur')
            return

        expr_txt, valeur_txt = texte.split('->', 1)
        expr = lire_expression(expr_txt)
        valeur = lire_expression(valeur_txt)
        L = limit(expr, x, valeur)
        label_resultat.config(text=f'Limite x --> {valeur} : {L}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

  
        
# DERIVEE ET INTEGRALE
def derivee():
    try:
        expr = lire_expression(entree.get())
        d = diff(expr, x)
        label_resultat.config(text=f"f\\'(x) = {d}")
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



def integrale():
    try:
        expr = lire_expression(entree.get())
        I = integrate(expr, x)
        label_resultat.config(text=f'∫f(x)dx = {I}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



# RESOLUTION D'EQUATIONS
def resoudre_inequation():
    try:
        expr = lire_expression(entree.get())
        solution = solve_univariate_inequality(expr, x)
        label_resultat.config(text=f'Solution : {solution}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

def resoudre():
    try:
        texte = entree.get()
        if '=' in texte:
            gauche, droite = texte.split('=', 1)
            eq = Eq(lire_expression(gauche),lire_expression(droite))
        else:
            eq = Eq(lire_expression(texte), 0)
        solutions = solve(eq, x)
        label_resultat.config(text=f'Solutions : {solutions}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



# FACTORISATION ET DEVELOPPEMENT
def developper():
    try:
        expr = lire_expression(entree.get())
        label_resultat.config(text=f'Développement : {expand(expr)}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')

def factoriser():
    try:
        expr = lire_expression(entree.get())
        label_resultat.config(text=f'Factorisation : {factor(expr)}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



# AFFICHAGE
def afficher_resultat(resultat): 
    if mode_scientifique.get(): 
        return f'{float(resultat):.6e}' 
    else: 
        return str(resultat.evalf())



# GRAPHIQUE ET VISUALISATION DE FONCTIONS
def tracer():
    try:
        expr = lire_expression(entree.get())

        f = lambdify(x, expr, 'numpy')

        X = np.linspace(-10, 10, 1000)
        Y = f(X)

        plt.figure(figsize=(6, 4))
        plt.plot(X, Y)
        plt.grid()
        plt.xlabel('x')
        plt.ylabel('f(x)')
        plt.title(f'f(x) = {expr}')
        plt.show()
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



# CHANGEMENT DE DEGRES A RADIANS
def preparer_expression(expr): 
    
    if mode_deg.get(): 
        expr = expr.replace('sin(', 'sin(pi/180*') 
        expr = expr.replace('cos(', 'cos(pi/180*') 
        expr = expr.replace('tan(', 'tan(pi/180*') 
    return expr



# FONCTION HISTORIQUE
def calculer():
    try:
        expression = entree.get()
        resultat = lire_expression(expression)

        historique.append(f'{expression} = {resultat}')
        zone_historique.delete(0, tk.END)
        for ligne in historique:
            zone_historique.insert(tk.END, ligne)

        label_resultat.config(text=f'Résultat : {afficher_resultat(resultat)}')
    except Exception as e:
        label_resultat.config(text=f'Erreur : {e}')



#------
# MAIN
#------



fenetre = tk.Tk()
fenetre.title('Calculatrice scientifique V2.0')
fenetre.attributes('-fullscreen', True)
fenetre.bind('<Escape>', confirmer_fermeture)

fenetre.protocol('WM_DELETE_WINDOW', confirmer_fermeture)

historique = []

zone_historique = tk.Listbox(fenetre, width=50, height=8) 
zone_historique.pack(pady=10)

entree = tk.Entry(fenetre, font=('Arial', 18), width=30)
entree.pack(pady=10)


mode_deg = tk.BooleanVar(value=True)
tk.Checkbutton(fenetre, text='Mode degré', variable=mode_deg).pack()
mode_scientifique = tk.BooleanVar(value=False)
tk.Checkbutton(fenetre, text='Écriture scientifique', variable=mode_scientifique).pack()

tk.Button(fenetre, text='=', font=('Arial', 16),command=calculer).pack(pady=10)
tk.Button(fenetre, text='Tracer', command=tracer).pack(pady=5)
tk.Button(fenetre, text='Développer', command=developper).pack()
tk.Button(fenetre, text='Factoriser', command=factoriser).pack()
tk.Button(fenetre, text='Dérivée', command=derivee).pack()
tk.Button(fenetre, text='Intégrale', command=integrale).pack()
tk.Button(fenetre, text='Résoudre équation', command=resoudre).pack()
tk.Button(fenetre, text='Résoudre inéquation', command=resoudre_inequation).pack()
tk.Button(fenetre, text='Limite x→+∞', command=limite_inf).pack()
tk.Button(fenetre, text='Limite x→-∞', command=limite_moins_inf).pack()
tk.Button(fenetre, text='Limite x→0', command=limite_zero).pack()
tk.Button(fenetre, text='Limite personnalisée', command=limite_valeur).pack()

label_resultat = tk.Label(fenetre, text='Résultat :',font=('Arial', 16))
label_resultat.pack()

fenetre.mainloop()