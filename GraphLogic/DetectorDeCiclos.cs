using System.Collections.Generic;
using System.Linq;

public class DetectorDeCiclos
{
    private Dictionary<string, List<string>> grafo;
    private bool direcionado;
    private List<List<string>> ciclos; // Lista de ciclos encontrados

    public DetectorDeCiclos(Grafo grafo)
    {
        this.grafo = grafo.ObterListaAdjacencia();
        this.direcionado = grafo.EhDirecionado();
        this.ciclos = new List<List<string>>();
    }

    // Percorrer todos os vertices e executar a DFS em cada um
    public List<List<string>> EncontrarTodosOsCiclos()
    {
        ciclos.Clear();

        foreach (var vertice in grafo.Keys)
        {
            DFS(vertice, vertice, new List<string>(), new HashSet<string>());
        }

        return ciclos;
    }

    // Busca em profundidade para detecção de ciclos
    private void DFS(string atual, string origem, List<string> caminho, HashSet<string> visitados)
    {
        caminho.Add(atual); // Adicionar o vertice atual na lista com o caminho
        visitados.Add(atual); // Adicionar o vetice atual na lista de visitados

        foreach (var vizinho in grafo[atual])
        {
            // Evita voltar direto ao vértice anterior (no caso de grafos não direcionados)
            if (caminho.Count > 1 && vizinho == caminho[caminho.Count - 2]) continue;

            // Se voltou ao vértice de origem, e tem tamanho mínimo de ciclo
            if (vizinho == origem && caminho.Count >= (direcionado ? 2 : 3))
            {
                var ciclo = new List<string>(caminho);
                ciclo.Add(origem);

                // Normaliza ciclo para evitar duplicatas e variações rotacionais
                var normalizado = NormalizarCiclo(ciclo);
                if (!ciclos.Any(c => Enumerable.SequenceEqual(NormalizarCiclo(c), normalizado)))
                {
                    ciclos.Add(ciclo);
                }
            }
            else if (!visitados.Contains(vizinho))
            {
                // Continua recursivamente o DFS
                DFS(vizinho, origem, new List<string>(caminho), new HashSet<string>(visitados));
            }
        }
    }

    // Metodo para normalizar ciclos para evitar salvamento de ciclos iguais com rotação diferente
    private List<string> NormalizarCiclo(List<string> ciclo)
    {
        // Remove repetição final para normalização
        ciclo = ciclo.Take(ciclo.Count - 1).ToList();

        var rotacoes = new List<List<string>>();

        // Gera todas as rotações possíveis do ciclo
        for (int i = 0; i < ciclo.Count; i++)
        {
            var rot = ciclo.Skip(i).Concat(ciclo.Take(i)).ToList();
            rotacoes.Add(rot);

            // Também considera rotações reversas se não for direcionado
            if (!direcionado)
                rotacoes.Add(rot.AsEnumerable().Reverse().ToList());
        }

        // Retorna a menor rotação organizada como forma canônica
        return rotacoes.OrderBy(r => string.Join(",", r)).First();
    }
}
