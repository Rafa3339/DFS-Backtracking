using System;
using System.Collections.Generic;

public class Grafo
{
    private Dictionary<string, List<string>> adjacencia; // Lista de adjacência do grafo
    private bool direcionado; // Flag indicando se o grafo é direcionado

    public Grafo(bool direcionado)
    {
        this.direcionado = direcionado;
        adjacencia = new Dictionary<string, List<string>>();
    }

    public void AdicionarAresta(string origem, string destino)
    {
        // Adiciona o destino à lista da origem
        if (!adjacencia.ContainsKey(origem))
            adjacencia[origem] = new List<string>();
        adjacencia[origem].Add(destino);

        // Se não for direcionado, adiciona a aresta inversa
        if (!direcionado)
        {
            if (!adjacencia.ContainsKey(destino))
                adjacencia[destino] = new List<string>();
            adjacencia[destino].Add(origem);
        }
    }

    public void MostrarGrafo()
    {
        // Exibe a lista de adjacência
        foreach (var par in adjacencia)
        {
            Console.Write($"{par.Key} -> ");
            foreach (var vizinho in par.Value)
                Console.Write($"{vizinho} ");
            Console.WriteLine();
        }
    }

    public bool ExisteAresta(string origem, string destino)
    {
        // Verifica se uma aresta existe
        return adjacencia.ContainsKey(origem) && adjacencia[origem].Contains(destino);
    }

    public Dictionary<string, List<string>> ObterListaAdjacencia()
    {
        return adjacencia;
    }

    public bool EhDirecionado()
    {
        return direcionado;
    }
}
