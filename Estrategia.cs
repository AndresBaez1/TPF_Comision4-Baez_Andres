using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepSpace
{
    class Estrategia
    {
        public Movimiento CalcularMovimiento(ArbolGeneral<Planeta> arbol)
		{
		    // Obtener el nodo que representa el planeta del Bot
		    return ObtenerMejorMovimientoDelBot(arbol);	
		   
		}
        
        private Movimiento ObtenerMejorMovimientoDelBot(ArbolGeneral<Planeta> arbol)
		{
		    // Obtener todos los nodos que representan planetas del Bot
		    List<ArbolGeneral<Planeta>> nodosDelBot = ObtenerNodosDelBot(arbol);
		    ArbolGeneral<Planeta> objetivo = null;
		    
		    		    
		    int poblacionObtenida;
		    // Elegir el mejor nodo basándose en la distancia al jugador y la capacidad de conquistar rápidamente
		    ArbolGeneral<Planeta> mejorNodo = null;
		    double mejorPuntuacion = double.MinValue;
		    double puntuacion;
		
		    foreach (ArbolGeneral<Planeta> nodoBot in nodosDelBot)
		    {
		        		
		        // Obtener todos los nodos adyacentes al nodo del Bot
		        List<ArbolGeneral<Planeta>> nodosAdyacentes = ObtenerNodosAdyacentes(arbol, nodoBot);
		        
		        foreach (ArbolGeneral<Planeta> nodoAdyacente in nodosAdyacentes)
		        {
		        	// Calcular la distancia al jugador
			        ArbolGeneral<Planeta> nodoJugadorCercano = ObtenerJugadorMasCercano(arbol, nodoAdyacente);
			        int distanciaAlJugador = CalcularDistanciaAlJugador(arbol, nodoBot, nodoJugadorCercano);
			        puntuacion = 0;
			        
			        if(nodoAdyacente.getDatoRaiz().EsPlanetaNeutral()){
			        	int poblacionAdyacente = nodoAdyacente.getDatoRaiz().Poblacion();
		           		int poblacionBot = nodoBot.getDatoRaiz().Poblacion();
						poblacionObtenida = poblacionBot - poblacionAdyacente ;		           			
		            	// Calcular la puntuación considerando la distancia al jugador y la población del planeta enemigo
		            	puntuacion = CalcularPuntuacion(distanciaAlJugador, (poblacionObtenida));
			        }
		            
		            if (nodoAdyacente.getDatoRaiz().EsPlanetaDeLaIA()){
		            	puntuacion = puntuacion-int.MaxValue; //Si es planeta de la IA no es tan importante
		            }
		            if (nodoAdyacente.getDatoRaiz().EsPlanetaDelJugador()){
			            poblacionObtenida = 9999999;	//Forzamos que el criterio de la poblacion para que deje de ser tan relevante.	           			
		            	// Calcular la puntuación considerando la distancia al jugador y la población del planeta enemigo
		            	puntuacion = CalcularPuntuacion(distanciaAlJugador, (poblacionObtenida));
		            }
		            // Elegir el adyacente con mayor puntuación
		            if (puntuacion >= mejorPuntuacion)
		            {
		            	objetivo = nodoAdyacente;
		                mejorPuntuacion = puntuacion;
		                mejorNodo = nodoBot;
		            }
		        }
		    }
		    
		    if(mejorNodo != null && objetivo != null){
		      	Movimiento movimiento = new Movimiento(mejorNodo.getDatoRaiz(), objetivo.getDatoRaiz());
		    	return movimiento;
		    }
		    
		    return null;
		    
		}
        
        private List<ArbolGeneral<Planeta>> ObtenerNodosDelBot(ArbolGeneral<Planeta> arbol)
		{
		    List<ArbolGeneral<Planeta>> nodosDelBot = new List<ArbolGeneral<Planeta>>();
		
		    if (arbol == null)
		    {
		        return nodosDelBot;
		    }
		
		    Cola<ArbolGeneral<Planeta>> cola = new Cola<ArbolGeneral<Planeta>>();
		    cola.encolar(arbol);
		
		    while (!cola.esVacia())
		    {
		        ArbolGeneral<Planeta> nodoActual = cola.desencolar();
		
		        // Verificar si el nodo actual pertenece a la IA (Bot)
		        if (nodoActual.getDatoRaiz().EsPlanetaDeLaIA())
		        {
		            nodosDelBot.Add(nodoActual);
		        }
		
		        // Agregar todos los hijos del nodo actual a la cola
		        foreach (ArbolGeneral<Planeta> hijo in nodoActual.getHijos())
		        {
		            cola.encolar(hijo);
		        }
		    }
		
		    return nodosDelBot;
		}
        
        private List<ArbolGeneral<Planeta>> ObtenerNodosDelJugador(ArbolGeneral<Planeta> arbol)
		{
		    List<ArbolGeneral<Planeta>> nodosDelBot = new List<ArbolGeneral<Planeta>>();
		
		    if (arbol == null)
		    {
		        return nodosDelBot;
		    }
		
		    Cola<ArbolGeneral<Planeta>> cola = new Cola<ArbolGeneral<Planeta>>();
		    cola.encolar(arbol);
		
		    while (!cola.esVacia())
		    {
		        ArbolGeneral<Planeta> nodoActual = cola.desencolar();
		
		        // Verificar si el nodo actual pertenece a la IA (Bot)
		        if (nodoActual.getDatoRaiz().EsPlanetaDelJugador())
		        {
		            nodosDelBot.Add(nodoActual);
		        }
		
		        // Agregar todos los hijos del nodo actual a la cola
		        foreach (ArbolGeneral<Planeta> hijo in nodoActual.getHijos())
		        {
		            cola.encolar(hijo);
		        }
		    }
		
		    return nodosDelBot;
		}
        
        private ArbolGeneral<Planeta> ObtenerJugadorMasCercano(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodoBot)
		{
		    List<ArbolGeneral<Planeta>> nodosJugador = ObtenerNodosDelJugador(arbol);
		
		    ArbolGeneral<Planeta> jugadorMasCercano = null;
		    int distanciaMinima = int.MaxValue;
		
		    foreach (ArbolGeneral<Planeta> nodoJugador in nodosJugador)
		    {
		        int distancia = CalcularDistanciaEntreNodos(arbol, nodoBot, nodoJugador);
		
		        if (distancia < distanciaMinima)
		        {
		            distanciaMinima = distancia;
		            jugadorMasCercano = nodoJugador;
		        }
		    }
		
		    return jugadorMasCercano;
		}
        
        private int CalcularDistanciaEntreNodos(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodo1, ArbolGeneral<Planeta> nodo2)
		{
		    int profundidadNodo1 = ObtenerProfundidad(arbol, nodo1);
		    int profundidadNodo2 = ObtenerProfundidad(arbol, nodo2);
		
		    // Calcular la distancia como la suma de las profundidades menos el doble de la profundidad del ancestro común.
		    return (profundidadNodo1 + profundidadNodo2) - (2 * ObtenerProfundidadDelAncestroComun(arbol, nodo1, nodo2));
		}
        
        private int ObtenerProfundidad(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodo)
		{
		    int profundidad = 0;
		
		    while (nodo != null && nodo != arbol)
		    {
		        nodo = ObtenerPadre(arbol, nodo);
		        profundidad++;
		    }
		
		    return profundidad;
		}
		
		private int ObtenerProfundidadDelAncestroComun(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodo1, ArbolGeneral<Planeta> nodo2)
		{
		    // Encontrar el ancestro común más profundo
		    HashSet<ArbolGeneral<Planeta>> ancestrosNodo1 = ObtenerAncestros(arbol, nodo1);
		    
		    while (nodo2 != null)
		    {
		        if (ancestrosNodo1.Contains(nodo2))
		        {
		            return ObtenerProfundidad(arbol, nodo2);
		        }
		
		        nodo2 = ObtenerPadre(arbol, nodo2);
		    }
		
		    return -1; // Valor negativo indica que no hay ancestro común
		}
		
		private HashSet<ArbolGeneral<Planeta>> ObtenerAncestros(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodo)
		{
		    HashSet<ArbolGeneral<Planeta>> ancestros = new HashSet<ArbolGeneral<Planeta>>();
		    
		    while (nodo != null)
		    {
		        ancestros.Add(nodo);
		        nodo = ObtenerPadre(arbol, nodo);
		    }
		
		    return ancestros;
		}
		
		private int CalcularDistanciaAlJugador(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodoAdyacente, ArbolGeneral<Planeta> nodoJugador)
		{
		    // Utilizar una búsqueda en amplitud (BFS) para calcular la distancia entre el nodo del bot y el nodo del jugador
		    Cola<Tuple<ArbolGeneral<Planeta>, int>> cola = new Cola<Tuple<ArbolGeneral<Planeta>, int>>();
		    HashSet<ArbolGeneral<Planeta>> visitados = new HashSet<ArbolGeneral<Planeta>>();
		
		    cola.encolar(new Tuple<ArbolGeneral<Planeta>, int>(nodoAdyacente, 0));
		    visitados.Add(nodoAdyacente);
		
		    while (!cola.esVacia())
		    {
		        var actualTuple = cola.desencolar();
		        ArbolGeneral<Planeta> actual = actualTuple.Item1;
		        int distancia = actualTuple.Item2;
		
		        if (actual == nodoJugador)
		        {
		            return distancia; // Se encontró el nodo del jugador, devolver la distancia
		        }
		
		        foreach (ArbolGeneral<Planeta> hijo in actual.getHijos())
		        {
		            if (!visitados.Contains(hijo))
		            {
		                cola.encolar(new Tuple<ArbolGeneral<Planeta>, int>(hijo, distancia + 1));
		                visitados.Add(hijo);
		            }
		        }
		    }
		
		    // Si no se encuentra el nodo del jugador, devolver un valor que indique que no hay conexión
		    return -1;
		}
		
		private ArbolGeneral<Planeta> ObtenerPadre(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodoBuscado)
		{
		    if (arbol == null || arbol == nodoBuscado)
		    {
		        return null;  // El árbol es nulo o el nodo buscado es la raíz, por lo que no tiene un padre
		    }
		
		    foreach (ArbolGeneral<Planeta> hijo in arbol.getHijos())
		    {
		        if (hijo == nodoBuscado)
		        {
		            return arbol;  // Se encontró el padre
		        }
		
		        // Buscar recursivamente en los hijos del hijo actual
		        ArbolGeneral<Planeta> padreEnSubarbol = ObtenerPadre(hijo, nodoBuscado);
		        if (padreEnSubarbol != null)
		        {
		            return padreEnSubarbol;  // Se encontró el padre en el subárbol
		        }
		    }
		
		    return null;  // No se encontró el nodo buscado en los hijos del árbol actual
		}
				
		private double CalcularPuntuacion(int distanciaAlJugador, int poblacionObtenida)
		{
						
			// Ajusta los pesos según tus necesidades, en este caso se prioriza la distancia en 60%
    		double pesoDistancia = 0.6;  // Puedes experimentar con diferentes valores
    		double pesoPoblacion = 0.4;  // Puedes experimentar con diferentes valores

		    // Calcula la puntuación combinando la distancia y la población
		    // En este caso, se suma la población, ya que queremos que una población más grande incremente la puntuación
		    return pesoDistancia / (1.0 + distanciaAlJugador) + (pesoPoblacion * poblacionObtenida);
		}

		
		private List<ArbolGeneral<Planeta>> ObtenerNodosAdyacentes(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodo)
		{
		    // Implementar la lógica para obtener todos los nodos adyacentes (padre e hijos) al nodo dado
		    // Puedes utilizar algoritmos de recorrido en el árbol.
		
		    List<ArbolGeneral<Planeta>> nodosAdyacentes = new List<ArbolGeneral<Planeta>>();
		
		    if (nodo != null)
		    {
		        // Agregar los hijos
		        if(nodo.getHijos() != null){
		        	nodosAdyacentes.AddRange(nodo.getHijos());
		        }
		        // Agregar el padre
		        ArbolGeneral<Planeta> padre = ObtenerPadre(arbol, nodo);
		        if (padre != null)
		        {
		            nodosAdyacentes.Add(padre);
		        }
		    }
		
		    return nodosAdyacentes;
		}
		
        public string Consulta1(ArbolGeneral<Planeta> arbol)
        {
        	//Calcula y retorna un texto con la distancia del camino que existe entre el planeta del Bot y la raíz.
            List<ArbolGeneral<Planeta>> nodosDelBot = ObtenerNodosDelBot(arbol);
		    string resultado = "";
		
		    foreach (ArbolGeneral<Planeta> nodoBot in nodosDelBot)
		    {
		        // Calcular la distancia desde el nodo del bot hasta la raíz
		        int distancia = ObtenerProfundidad(arbol, nodoBot);
		
		        // Agregar la información al resultado
		        resultado += "Distancia desde la raíz hasta el planeta del Bot: " + distancia + "\n";
		    }
		
		    return resultado;
        }

        public string Consulta2(ArbolGeneral<Planeta> arbol)
        {
        	//Retorna un texto con el listado de los planetas ubicados en todos los descendientes del nodo que contiene al planeta del Bot.
        	string planetasDescendientes = "";
            List<ArbolGeneral<Planeta>> NodosDelBot = ObtenerNodosDelBot(arbol);
            foreach(ArbolGeneral<Planeta> Nodo in NodosDelBot)
            {
            	List<Planeta> descendientes = ObtenerDescendientes(arbol, Nodo);

            	
            	foreach(Planeta p in descendientes){
            		planetasDescendientes = planetasDescendientes + 
            		("Planeta: (x="+ p.position.X.ToString()+"; y="+ p.position.Y.ToString()+ "); ");
            	}
            }

            return "Descendientes del Bot: " + planetasDescendientes;
        }

        public string Consulta3(ArbolGeneral<Planeta> arbol)
        {
        	//Calcula y retorna en un texto la población total y promedio por cada nivel del árbol.
            Dictionary<int, Tuple<int, int>> poblacionPorNivel = CalcularPoblacionPorNivel(arbol);

            string resultado = "Población por nivel:\n";

            foreach (var nivel in poblacionPorNivel)
            {
                resultado +="Nivel "+nivel.Key+": Población total = "+nivel.Value.Item1+", Población promedio = "+nivel.Value.Item2+"\n";
            }

            return resultado;
        }


        private List<Planeta> ObtenerDescendientes(ArbolGeneral<Planeta> arbol, ArbolGeneral<Planeta> nodoActual)
        {
           
            if (nodoActual != null)
            {
                List<Planeta> descendientes = new List<Planeta>();
                ObtenerDescendientesRecursivo(nodoActual, descendientes);
                return descendientes;
            }

            return new List<Planeta>();
        }

        private void ObtenerDescendientesRecursivo(ArbolGeneral<Planeta> nodo, List<Planeta> descendientes)
        {
            descendientes.Add(nodo.getDatoRaiz());

            foreach (ArbolGeneral<Planeta> hijo in nodo.getHijos())
            {
                ObtenerDescendientesRecursivo(hijo, descendientes);
            }
        }

        private Dictionary<int, Tuple<int, int>> CalcularPoblacionPorNivel(ArbolGeneral<Planeta> arbol)
        {
        	//El diccionario tiene el nivel en la clave y una tupla con la población total en el primer item y la cantidad de nodos en el segundo item.
            Dictionary<int, Tuple<int, int>> poblacionPorNivel = new Dictionary<int, Tuple<int, int>>();
            CalcularPoblacionPorNivelRecursivo(arbol, 0, poblacionPorNivel);

            return poblacionPorNivel;
        }

        private void CalcularPoblacionPorNivelRecursivo(ArbolGeneral<Planeta> nodo, int nivel, Dictionary<int, Tuple<int, int>> poblacionPorNivel)
        {
            if (!poblacionPorNivel.ContainsKey(nivel))
            {
                poblacionPorNivel[nivel] = Tuple.Create(0, 0);
            }

            poblacionPorNivel[nivel] = Tuple.Create(poblacionPorNivel[nivel].Item1 + nodo.getDatoRaiz().Poblacion(), poblacionPorNivel[nivel].Item2 + 1);

            foreach (ArbolGeneral<Planeta> hijo in nodo.getHijos())
            {
                CalcularPoblacionPorNivelRecursivo(hijo, nivel + 1, poblacionPorNivel);
            }
        }

    }
}
