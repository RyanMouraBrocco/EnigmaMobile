using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class InfoPerguntaRespostaPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        Resposta resposta = null;
        Pergunta pergunta = null;
        bool isbusy = false;
        public InfoPerguntaRespostaPage(Resposta resposta, Pergunta pergunta)
        {
            InitializeComponent();
            this.resposta = resposta;
            this.pergunta = pergunta;
            if (resposta == null)
            {
                this.Title = "Info Pergunta";
            }
            else
            {
                this.Title = "Info Resposta";
            }
            CarregarInfoAsync();
        }

        public async void CarregarInfoAsync()
        {
            frm_carregando.IsVisible = true;
            try
            {
                int positivos = 0, negativos = 0, denucias = 0;
                if (resposta == null)
                {
                    var resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/allpositivos/" + pergunta.ID);
                    positivos = JsonConvert.DeserializeObject<int>(resp);
                    resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/allnegativos/" + pergunta.ID);
                    negativos = JsonConvert.DeserializeObject<int>(resp);
                    resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/allDenuncias/" + pergunta.ID);
                    denucias = JsonConvert.DeserializeObject<int>(resp);
                    txt_Titulo.Text = pergunta.Titulo;
                    if (pergunta.Visibilidade == true)
                    {
                        SwVisibilidade.IsToggled = true;
                    }
                    else
                    {
                        SwVisibilidade.IsToggled = false;
                    }
                }
                else
                {
                    var resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/allpositivos/" + resposta.ID);
                    positivos = JsonConvert.DeserializeObject<int>(resp);
                    resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/allnegativos/" + resposta.ID);
                    negativos = JsonConvert.DeserializeObject<int>(resp);
                    resp = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/allDenuncias/" + resposta.ID);
                    denucias = JsonConvert.DeserializeObject<int>(resp);
                    txt_Titulo.Text = resposta.Titulo;
                    if (resposta.Visibilidade == true)
                    {
                        SwVisibilidade.IsToggled = true;
                    }
                    else
                    {
                        SwVisibilidade.IsToggled = false;
                    }
                }
                txtlikes.Text = positivos.ToString();
                txtdislikes.Text = negativos.ToString();
                txtdenucnias.Text = denucias.ToString();
            }
            catch
            {
                await DisplayAlert("Erro", "Erro de conexão", "OK");
                await Navigation.PopModalAsync();
            }
            frm_carregando.IsVisible = false;

        }


        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                frm_carregando.IsVisible = true;
                try
                {
                    if (resposta == null)
                    {
                        var resp = await client.GetStringAsync(server.Servidor + "/api/Pergunta/byid/" + pergunta.ID);
                        Pergunta p = JsonConvert.DeserializeObject<Pergunta>(resp);
                        await Navigation.PushAsync(new PerguntaViewPage(p));
                    }
                    else
                    {
                        var resp = await client.GetStringAsync(server.Servidor + "/api/Pergunta/byid/" + resposta.Pergunta.ID);
                        Pergunta p = JsonConvert.DeserializeObject<Pergunta>(resp);
                        await Navigation.PushAsync(new PerguntaViewPage(p));
                    }

                }
                catch
                {
                    await DisplayAlert("Erro", "Erro de conexão", "OK");
                }
                frm_carregando.IsVisible = false;
                isbusy = false;
            }
           
        }

        void Sair_Clicked(object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                Navigation.PopModalAsync();
            }
        }

        async void Handle_ToggledAsync(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (isbusy==false)
            {
                try
                {
                    if (SwVisibilidade.IsToggled == true)
                    {
                        if (resposta == null)
                        {
                            pergunta.Visibilidade = true;
                            var valor = JsonConvert.SerializeObject(pergunta);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resp = null;
                            resp = await client.PostAsync(server.Servidor + "/api/Pergunta/alterar", conteudo);
                            if (!resp.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                SwVisibilidade.IsToggled = false;
                                await Navigation.PopModalAsync();
                            }
                        }
                        else
                        {
                            resposta.Visibilidade = true;
                            var valor = JsonConvert.SerializeObject(resposta);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resp = null;
                            resp = await client.PostAsync(server.Servidor + "/api/Resposta/alterar", conteudo);
                            if (!resp.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                SwVisibilidade.IsToggled = false;
                                await Navigation.PopModalAsync();
                            }
                        }
                    }
                    else
                    {
                        if (resposta == null)
                        {
                            pergunta.Visibilidade = false;
                            var valor = JsonConvert.SerializeObject(pergunta);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resp = null;
                            resp = await client.PostAsync(server.Servidor + "/api/Pergunta/alterar", conteudo);
                            if (!resp.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                SwVisibilidade.IsToggled = true;
                                await Navigation.PopModalAsync();
                            }
                        }
                        else
                        {
                            resposta.Visibilidade = false;
                            var valor = JsonConvert.SerializeObject(resposta);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resp = null;
                            resp = await client.PostAsync(server.Servidor + "/api/Resposta/alterar", conteudo);
                            if (!resp.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                SwVisibilidade.IsToggled = true;
                                await Navigation.PopModalAsync();
                            }
                        }
                    }
                }
                catch
                {
                    await DisplayAlert("Erro", "Erro de conexão", "OK");
                    await Navigation.PopModalAsync();
                }

            }

        }
    }
}
