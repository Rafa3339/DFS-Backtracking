using System;
using System.Collections.Generic;

class GeradorDeGrafos
{
    private Random random; // Gerador de números aleatórios

    public GeradorDeGrafos(int? semente = null)
    {
        // Usa semente se fornecida, para resultados previsíveis
        random = semente.HasValue ? new Random(semente.Value) : new Random();
    }

    private string GerarNomeVertice(int index)
    {
        // Gera nomes de vértices como A, B, ..., Z, AA, AB, etc.
        string nome = "";
        while (true)
        {
            nome = (char)('A' + (index % 26)) + nome;
            index /= 26;
            if (index == 0) break;
            index--;
        }
        return nome;
    }

    public Grafo GerarGrafoAleatorio(bool direcionado, int totalVertices, int totalArestas)
    {
        var grafo = new Grafo(direcionado);
        var nomesVertices = new List<string>();

        // Gera os nomes dos vértices
        for (int i = 0; i < totalVertices; i++)
            nomesVertices.Add(GerarNomeVertice(i));
            
        foreach (string nome in nomesVertices)
        {
            if (!grafo.ObterListaAdjacencia().ContainsKey(nome))
                grafo.ObterListaAdjacencia()[nome] = new List<string>();
        }

        var arestasAdicionadas = new HashSet<(string, string)>();
        int tentativas = 0;
        int maxTentativas = totalArestas * 3;

        // Adiciona arestas aleatórias evitando duplicações e laços
        while (arestasAdicionadas.Count < totalArestas && tentativas < maxTentativas)
        {
            string origem = nomesVertices[random.Next(totalVertices)];
            string destino = nomesVertices[random.Next(totalVertices)];

            if (origem == destino) { tentativas++; continue; }

            var par = direcionado
                ? (origem, destino)
                : (origem.CompareTo(destino) < 0 ? (origem, destino) : (destino, origem));

            if (arestasAdicionadas.Contains(par)) { tentativas++; continue; }

            arestasAdicionadas.Add(par);
            grafo.AdicionarAresta(origem, destino);
        }

        return grafo;
    }
}
