using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DFS_Backtracking;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _pixel;

    private Grafo grafo;
    private Dictionary<string, Vector2> posicoes;

    private Rectangle botaoDetectarCiclos;
    private Rectangle botaoAnterior;
    private Rectangle botaoProximo;
    private List<List<string>> ciclosDetectados;
    private int cicloAtual = 0;
    private MouseState mouseAnterior;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1000;
        _graphics.PreferredBackBufferHeight = 700;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        int screenWidth = GraphicsDevice.Viewport.Width;
        int screenHeight = GraphicsDevice.Viewport.Height;

        // Posição mais baixa para evitar sobreposição com vértices
        botaoDetectarCiclos = new Rectangle(screenWidth / 2 - 100, screenHeight - 80, 200, 40);
        botaoAnterior = new Rectangle(screenWidth / 2 - 160, screenHeight - 80, 40, 40);
        botaoProximo = new Rectangle(screenWidth / 2 + 120, screenHeight - 80, 40, 40);

        base.Initialize();
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("DefaultFont");

        // Gerador do grafo
        var gerador = new GeradorDeGrafos(); // Pode ter semente para geração igual
        grafo = gerador.GerarGrafoAleatorio(
            direcionado: true,
            totalVertices: 12,
            totalArestas: 30);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        GerarPosicoesCirculares();
    }

    private void GerarPosicoesCirculares()
    {
        posicoes = new Dictionary<string, Vector2>();
        var adj = grafo.ObterListaAdjacencia();
        HashSet<string> vertices = new HashSet<string>();

        foreach (var (origem, vizinhos) in adj)
        {
            vertices.Add(origem);
            foreach (var destino in vizinhos)
                vertices.Add(destino);
        }

        int n = vertices.Count;
        float raio = 200f;
        Vector2 centro = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2 - 40);

        int i = 0;
        foreach (var vertice in vertices)
        {
            float angulo = MathHelper.TwoPi * i / n;
            float x = centro.X + raio * (float)Math.Cos(angulo);
            float y = centro.Y + raio * (float)Math.Sin(angulo);
            posicoes[vertice] = new Vector2(x, y);
            i++;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed && mouseAnterior.LeftButton == ButtonState.Released)
        {
            Point click = new Point(mouse.X, mouse.Y);

            if (botaoDetectarCiclos.Contains(click))
            {
                var detector = new DetectorDeCiclos(grafo);
                ciclosDetectados = detector.EncontrarTodosOsCiclos();
                cicloAtual = 0;
            }
            else if (ciclosDetectados != null && ciclosDetectados.Count > 0)
            {
                if (botaoAnterior.Contains(click))
                {
                    cicloAtual = (cicloAtual - 1 + ciclosDetectados.Count) % ciclosDetectados.Count;
                }
                else if (botaoProximo.Contains(click))
                {
                    cicloAtual = (cicloAtual + 1) % ciclosDetectados.Count;
                }
            }
        }

        mouseAnterior = mouse;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();

        // Ciclo destacado
        if (ciclosDetectados != null && ciclosDetectados.Count > 0)
        {
            var ciclo = ciclosDetectados[cicloAtual];
            for (int i = 0; i < ciclo.Count - 1; i++)
            {
                string origem = ciclo[i];
                string destino = ciclo[i + 1];

                if (posicoes.ContainsKey(origem) && posicoes.ContainsKey(destino))
                {
                    DrawArrow(posicoes[origem], posicoes[destino], Color.Red, 4f);
                }
            }
        }

        // Arestas do grafo
        foreach (var (origem, vizinhos) in grafo.ObterListaAdjacencia())
        {
            if (!posicoes.ContainsKey(origem)) continue;
            Vector2 posOrigem = posicoes[origem];

            foreach (var destino in vizinhos)
            {
                if (!posicoes.ContainsKey(destino)) continue;
                Vector2 posDestino = posicoes[destino];
                DrawArrow(posOrigem, posDestino, Color.Gray, 2f);
            }
        }

        // Vértices do grafo
        foreach (var (nome, pos) in posicoes)
        {
            float tamanho = 20f;
            Rectangle circuloRect = new Rectangle((int)(pos.X - tamanho / 2), (int)(pos.Y - tamanho / 2), (int)tamanho, (int)tamanho);
            _spriteBatch.Draw(_pixel, circuloRect, Color.SkyBlue);

            Vector2 textoPos = pos - _font.MeasureString(nome) / 2;
            _spriteBatch.DrawString(_font, nome, textoPos, Color.Black);
        }

        // Botões
        _spriteBatch.Draw(_pixel, botaoDetectarCiclos, Color.LightGray);
        _spriteBatch.DrawString(_font, "Detectar Ciclos", new Vector2(botaoDetectarCiclos.X + 10, botaoDetectarCiclos.Y + 10), Color.Black);

        if (ciclosDetectados != null)
        {
            _spriteBatch.Draw(_pixel, botaoAnterior, Color.LightBlue);
            _spriteBatch.DrawString(_font, "<", new Vector2(botaoAnterior.X + 12, botaoAnterior.Y + 5), Color.Black);

            _spriteBatch.Draw(_pixel, botaoProximo, Color.LightBlue);
            _spriteBatch.DrawString(_font, ">", new Vector2(botaoProximo.X + 12, botaoProximo.Y + 5), Color.Black);

            _spriteBatch.DrawString(_font, $"Ciclos: {ciclosDetectados.Count}", new Vector2(20, 20), Color.DarkRed);
            _spriteBatch.DrawString(_font, $"Ciclo {cicloAtual + 1}/{ciclosDetectados.Count}", new Vector2(20, 50), Color.DarkSlateGray);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }


    private void DrawArrow(Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 edge = end - start;
        float comprimento = edge.Length();

        if (comprimento < 1f) return; // evita divisão por zero

        Vector2 direcao = Vector2.Normalize(edge);

        float offsetFinal = 15f; // distância antes do centro do vértice de destino
        Vector2 destinoAjustado = end - direcao * offsetFinal;

        // Linha principal da aresta
        _spriteBatch.Draw(_pixel, new Rectangle((int)start.X, (int)start.Y, (int)(comprimento - offsetFinal), (int)thickness),
            null, color, (float)Math.Atan2(edge.Y, edge.X), Vector2.Zero, SpriteEffects.None, 0);

        // Desenhar seta somente se for direcionado
        if (grafo.EhDirecionado())
        {
            float largura = 6f;

            Vector2 p1 = destinoAjustado + new Vector2(-direcao.Y, direcao.X) * largura;
            Vector2 p2 = end;
            Vector2 p3 = destinoAjustado + new Vector2(direcao.Y, -direcao.X) * largura;

            DrawTriangle(p1, p2, p3, Color.Red); // agora em vermelho
        }
    }


    private void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
    {
        void DrawEdge(Vector2 a, Vector2 b)
        {
            Vector2 edge = b - a;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            _spriteBatch.Draw(_pixel, new Rectangle((int)a.X, (int)a.Y, (int)edge.Length(), 1), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        DrawEdge(p1, p2);
        DrawEdge(p2, p3);
        DrawEdge(p3, p1);
    }
}
