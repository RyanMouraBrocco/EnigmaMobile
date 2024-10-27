using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class RealizarExercicioPage : ContentPage
    {
        HttpClient client = new HttpClient();
        Server server = new Server();
        Exercicio Gabarito = new Exercicio();
        Exercicio Realizado = new Exercicio();
        int[] sequencia = new int[1];
        int[] sequenciaalternativa = new int[1];
        int questaoatual = 0;
        bool[] checks = new bool[1];
        int escolha = 0;
        Entry[] entrys = new Entry[0];
        bool enviar = false;
        int[] escolhas = new int[1];
        bool isbusy = false;
        bool continuar = true;
 
        public RealizarExercicioPage(Exercicio exercicio)
        {
            InitializeComponent();
            this.Gabarito = exercicio;
            sequencia = new int[Gabarito.Questao.Count];
            EmbaralharQuestao();
            ExibirQuestao();
            this.Realizado.ID = exercicio.ID;
            this.Realizado.Usuario = UsuarioAtual.ID;
            this.Realizado.AleatorioQuestao = exercicio.AleatorioQuestao;
            this.Realizado.Conteudo = exercicio.Conteudo;
            this.Realizado.Tipo = exercicio.Tipo;
            this.Realizado.Descricao = exercicio.Descricao;
            this.Realizado.Questao = new List<Questao>();
        }
        void ExibirQuestao()
        {
            Questao atual = new Questao();
            foreach (var item in Gabarito.Questao)
            {
                if (item.Ordem == sequencia[questaoatual])
                {
                    atual = item;
                    break;
                }
            }
            stl_Alternativa.Children.Clear();
            stl_Img.Children.Clear();
            sequenciaalternativa = new int[atual.Alternativa.Count];
            if (atual.AleatorioAlternativa==true)
            {
                Random random = new Random();
                int valor = 0;
                List<int> lista = new List<int>();
                bool salvar = true;
                for (int i = 0; i < atual.Alternativa.Count; i++)            
                {                
                    valor = random.Next(1, atual.Alternativa.Count+1);                
                    while (lista.Contains(valor))                
                   {                                        
                        if (lista.Count == atual.Alternativa.Count) // quando ficar em loop infinito vou quebrar o laço  
                        {
                                salvar = false;
                                break;
                               
                        }                 
                       else      
                        {
                            valor = random.Next(1,atual.Alternativa.Count+1); 
                        }
                                           
                   } 
                    if (salvar)
                    {
                        lista.Add(valor);
                        sequenciaalternativa[i] = valor;
                    }

                }   
            }
            else
            {
                int index = 0;
                foreach (var item in atual.Alternativa.OrderBy(x=>x.Ordem))
                {
                    sequenciaalternativa[index] = item.Ordem;
                    index += 1;
                }
            }
            if (atual.Tipo=="A")
            {
                checks = new bool[sequenciaalternativa.Length];
                Selecionar(atual);
            }
            else
            {
                stl_Alternativa.Children.Clear();
                int index = 0;
                foreach (var item in sequenciaalternativa)
                {
                    Entry entry = new Entry();
                    Alternativa alternativa = new Alternativa();
                    entrys = new Entry[atual.Alternativa.Count];
                    foreach (var I in atual.Alternativa)
                    {
                        if (sequenciaalternativa[index] == I.Ordem)
                        {
                            alternativa = I;
                            entry.HorizontalOptions = LayoutOptions.FillAndExpand;
                            entry.Placeholder="Valor: " + I.Ordem.ToString();
                            entry.Unfocused += (sender, e) => 
                            {
                                Entry este = sender as Entry;
                                entrys[Convert.ToInt32(este.Placeholder.Substring(este.Placeholder.Length-1,1))-1]= este;
                            };
                            index += 1;
                            stl_Alternativa.Children.Add(entry);
                            break;
                        }
                    }
                }

            }
            lblPergunta.Text = atual.Pergunta;
            if (atual.Imagem!=null)
            {
                foreach (var item in atual.Imagem)
                {
                    Image img = new Image();
                    MemoryStream memoryStream = new MemoryStream(item._Imagem);
                    img.Source = ImageSource.FromStream(() => { return memoryStream; });
                    stl_Img.Children.Add(img);
                    Task.Delay(1000);
                }
            }

        }

        void Selecionar(Questao atual)
        {
            stl_Alternativa.Children.Clear();
            int index = 0;
            escolhas = new int[atual.Alternativa.Count];
            foreach (var item in sequenciaalternativa)
            {
                Button button = new Button();
                Alternativa alternativa = new Alternativa();
                foreach (var I in atual.Alternativa)
                {
                    if (sequenciaalternativa[index] == I.Ordem)
                    {
                        alternativa = I;
                        if (checks[index] == true)
                        {
                            button.Text = index + 1 + "- (X) " + alternativa.Conteudo;
                            escolhas[index] = I.Ordem;
                        }
                        else
                        {
                            button.Text = index + 1 + "- ( ) " + alternativa.Conteudo;
                            escolhas[index] = I.Ordem;
                        }
                        button.Clicked += (sender, e) =>
                        {
                            if (isbusy==false)
                            {
                                Button este = sender as Button;
                                for (int i = 0; i < checks.Length; i++)
                                {
                                    checks[i] = false;
                                }
                                checks[Convert.ToInt32(este.Text.Trim().Substring(0, este.Text.IndexOf("-", StringComparison.Ordinal))) - 1] = true;
                                escolha = escolhas[Convert.ToInt32(este.Text.Trim().Substring(0, este.Text.IndexOf("-", StringComparison.Ordinal))) - 1];
                                Selecionar(atual);
                            }
                        };
                        index += 1;
                        button.VerticalOptions = LayoutOptions.FillAndExpand;
                        button.HorizontalOptions = LayoutOptions.FillAndExpand;
                        stl_Alternativa.Children.Add(button);
                        break;
                    }
                }
            }
        }

        void  EmbaralharQuestao()
        {
            if (Gabarito.AleatorioQuestao==true)
            {
                Random random = new Random();
                int valor = 0;
                List<int> lista = new List<int>();
                bool salvar = true;
                for (int i = 0; i < Gabarito.Questao.Count; i++)            
                {                
                    valor = random.Next(1, Gabarito.Questao.Count+1);                
                    while (lista.Contains(valor))                
                   {                                        
                        if (lista.Count == Gabarito.Questao.Count) // quando ficar em loop infinito vou quebrar o laço  
                        {
                            salvar = false;
                            break;
                        }
                        else
                        {
                            valor = random.Next(1, Gabarito.Questao.Count+1);
                        }               
                   }     
                    if (salvar)
                    {
                        lista.Add(valor);
                        sequencia[i] = valor;
                    }
                }            
                       
            }
            else
            {
                int index = 0;
                foreach (var item in Gabarito.Questao.OrderBy(x=>x.Ordem))
                {
                    sequencia[index] = item.Ordem;
                    index += 1;
                }
            }
        }

        bool adicionar = true;

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            if (continuar)
            {
                if (isbusy == false)
                {
                    isbusy = true;

                    Questao atual = new Questao();
                    foreach (var item in Gabarito.Questao)
                    {
                        if (item.Ordem == sequencia[questaoatual])
                        {
                            atual = item;
                            break;
                        }
                    }
                    if (atual.Tipo == "A")
                    {
                        bool validar = false;
                        for (int i = 0; i < checks.Length; i++)
                        {
                            if (checks[i])
                            {
                                validar = true;
                                break;
                            }
                        }
                        if (validar)
                        {
                            Alternativa esta = new Alternativa();
                            foreach (var item in atual.Alternativa)
                            {
                                if (escolha == item.Ordem)
                                {
                                    esta = item;
                                    break;
                                }
                            }
                            atual.Alternativa.Clear();
                            esta.Tipo = "R";
                            atual.Alternativa.Add(esta);
                            if (questaoatual + 1 < Gabarito.Questao.Count)
                            {
                                questaoatual += 1;
                                ExibirQuestao();
                            }
                            else
                            {

                                if (questaoatual + 1 == Gabarito.Questao.Count)
                                {
                                    enviar = true;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("ERRO", "Selecione uma alternativa", "OK");
                            adicionar = false;
                        }
                    }
                    else
                    {
                        bool validar = true;
                        for (int i = 0; i < entrys.Length; i++)
                        {
                            if (entrys[i] == null)
                            {
                                validar = false;
                                break;
                            }
                            else
                            {
                                if (entrys[i].Text.Trim() == "")
                                {
                                    validar = false;
                                    break;
                                }
                            }

                        }
                        if (validar == false)
                        {
                            await DisplayAlert("ERRO", "Preencha todos os campos", "OK");
                            adicionar = false;
                        }
                        else
                        {
                            List<Alternativa> alternativas = new List<Alternativa>();
                            int index = 0;
                            foreach (var item in atual.Alternativa)
                            {
                                item.Tipo = "R";
                                item.Conteudo = entrys[index].Text.Trim();
                                alternativas.Add(item);
                                index += 1;
                            }
                            atual.Alternativa.Clear();
                            atual.Alternativa = alternativas;
                            if (questaoatual + 1 < Gabarito.Questao.Count)
                            {
                                questaoatual += 1;
                                ExibirQuestao();
                            }
                            else
                            {
                                if (questaoatual + 1 == Gabarito.Questao.Count)
                                {
                                    enviar = true;
                                }
                            }
                        }
                    }
                    if (adicionar)
                    {
                        Realizado.Questao.Add(atual);
                    }
                    adicionar = true;
                }
            }
            if (enviar)
            {
                continuar = false;
                frm_carregando.IsVisible = true;
                isbusy = true;
                foreach (var item in entrys)
                {
                    item.IsEnabled = false;
                }
                try
                {
                    var valor = JsonConvert.SerializeObject(Realizado);
                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                    HttpResponseMessage resposta = null;
                    resposta = await client.PostAsync(server.Servidor + "/api/Exercicio/corrigir", conteudo);
                    if (!resposta.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                    }
                    else
                    {
                        var resp = await client.GetStringAsync(server.Servidor + "/api/Nota/ultimo/byusuarioexercicio/" + UsuarioAtual.ID + "/" + Realizado.ID);
                        Nota nota = JsonConvert.DeserializeObject<Nota>(resp);
                        await DisplayAlert("Nota", "Sua Nota foi : " + nota._Nota.ToString(), "OK");
                        await Navigation.PopAsync();
                    }
                }
                catch
                {
                    foreach (var item in entrys)
                    {
                        item.IsEnabled = true;
                    }
                    await DisplayAlert("ERRO", "Erro de conexão", "OK");
                }

                frm_carregando.IsVisible = false;
            }
                isbusy = false;

        }
    }
}
