using System;
using System.Collections.Generic;

namespace DeepSpace
{
	public class ArbolGeneral<T>
	{
		
		private T dato;
		private List<ArbolGeneral<T>> hijos = new List<ArbolGeneral<T>>();

		public ArbolGeneral(T dato) {
			this.dato = dato;
		}
	
		public T getDatoRaiz() {
			return this.dato;
		}
	
		public List<ArbolGeneral<T>> getHijos() {
			return hijos;
		}
	
		public void agregarHijo(ArbolGeneral<T> hijo) {
			this.getHijos().Add(hijo);
		}
	
		public void eliminarHijo(ArbolGeneral<T> hijo) {
			this.getHijos().Remove(hijo);
		}
	
		public bool esHoja() {
			return this.getHijos().Count == 0;
		}
	
		public int altura() {
			if (this.esHoja()) {
        		return 0; // La altura de una hoja es 0.
		    } else {
		        int alturaMaxima = 0;
		        foreach (var hijo in this.getHijos()) {
		            int alturaHijo = hijo.altura();
		            if (alturaHijo > alturaMaxima) {
		                alturaMaxima = alturaHijo;
		            }
		        }
		        return 1 + alturaMaxima; // La altura es 1 más la altura máxima de los hijos.
		    }
		}
	
		
		public int nivel(T dato) {
			return nivelAux(dato, 0); // Llamada al método auxiliar con el nivel inicial 0.
		}
		
		private int nivelAux(T dato, int nivelActual) {
		    if (this.getDatoRaiz().Equals(dato)) {
		        return nivelActual; // Se encontró el dato en el nivel actual.
		    } else {
		        foreach (var hijo in this.getHijos()) {
		            int nivelEncontrado = hijo.nivelAux(dato, nivelActual + 1);
		            if (nivelEncontrado != -1) {
		                return nivelEncontrado; // El dato fue encontrado en algún nivel.
		            }
		        }
		        return -1; // El dato no fue encontrado en este subárbol.
		    }
		}
	
	}
}