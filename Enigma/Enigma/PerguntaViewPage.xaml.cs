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
    public partial class PerguntaViewPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient(); 
        Pergunta pergunta = new Pergunta();
        Image[] positivos = new Image[1];
        Image[] negativos = new Image[1];

        public PerguntaViewPage(Pergunta pergunta)
        {
            InitializeComponent();
            this.pergunta = pergunta;
        }

        async Task CarregarPerguntaAsync()
        {
            try
            {
                var rsp = await client.GetStringAsync(server.Servidor+"/api/Pergunta/byid/"+pergunta.ID);
                this.pergunta = new Pergunta();
                this.pergunta = JsonConvert.DeserializeObject<Pergunta>(rsp);
                rsp = null;

                stlusuario.Children.Clear();
                stlrespostas.Children.Clear();
                stlimagenspergunta.Children.Clear();

                // Monstando a pergunta

                Usuario usuario = new Usuario();
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Usuario/byid/" + pergunta.Usuario);
                usuario = JsonConvert.DeserializeObject<Usuario>(resposta);
                resposta = null;
                var imgusuario = new ImageCircle
                {
                    WidthRequest = 100,
                    HeightRequest = 100,
                    Aspect = Aspect.AspectFill
                };
                await Task.Delay(1000);
                MemoryStream ms = new MemoryStream();
                //Add as info do usuario que realizou  a pergunta 
                if (usuario.Foto!=null)
                {
                    ms = new MemoryStream(usuario.Foto);
                    imgusuario.Source = ImageSource.FromStream(() => { return ms; });
                }
                else
                {
                    imgusuario.Source = "user.png";
                }
                stlusuario.Children.Add(imgusuario);
                stlusuario.Children.Add(new Label { Text = usuario.Nome, VerticalOptions = LayoutOptions.CenterAndExpand });
                lbltitulo.Text = pergunta.Titulo;
                lblpergunta.Text = pergunta.Texto;

                await Task.Delay(1000);
                // add as imagens da Pergunta
                foreach (var item in pergunta.Imagem)
                {
                    Image img = new Image();
                    ms = new MemoryStream(item._Imagem);
                    img.Source = ImageSource.FromStream(() => { return ms; });
                    stlimagenspergunta.Children.Add(img);
                    await Task.Delay(1000);
                }
                //Avaliacao do Usuario atual para a Pergunta
                resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/byid/" + UsuarioAtual.ID + "/" + pergunta.ID);
                Avaliacao perguntaaval = new Avaliacao();
                perguntaaval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                resposta = null;
                if (perguntaaval.ID != 0)
                {
                    if (perguntaaval._Avaliacao == true)
                    {
                        imgperguntalike.Source = "like_ativo.png";
                    }
                    else
                    {
                        imgperguntadislike.Source = "dislike_ativo.png";
                    }
                }
                TapGestureRecognizer tapPerguntaLike = new TapGestureRecognizer();
                tapPerguntaLike.Tapped += async (object sender, EventArgs e) =>
                {
                    Avaliacao aval = new Avaliacao
                    {
                        Denuncia = false,
                        Pergunta = new Pergunta { ID = pergunta.ID },
                        Resposta = null,
                        _Avaliacao = true,
                        Usuario = new Usuario { ID = UsuarioAtual.ID }
                    };
                    if (imgperguntadislike.Source.ToString() == "File: dislike_ativo.png")
                    {
                        resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/byid/" + UsuarioAtual.ID + "/" + aval.Pergunta.ID);
                        aval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                        resposta = null;
                        aval._Avaliacao = true;
                        var valor = JsonConvert.SerializeObject(aval);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/alterar", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            imgperguntalike.Source = "like_ativo.png";
                            imgperguntadislike.Source = "dislike.png";
                        }
                    }
                    else
                    {
                        var valor = JsonConvert.SerializeObject(aval);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/inserir", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            imgperguntalike.Source = "like_ativo.png";
                            imgperguntadislike.Source = "dislike.png";
                        }
                    }
                };
                imgperguntalike.GestureRecognizers.Add(tapPerguntaLike);
                TapGestureRecognizer tapPerguntaDislike = new TapGestureRecognizer();
                tapPerguntaDislike.Tapped += async (object sender, EventArgs e) =>
                {
                    Avaliacao aval = new Avaliacao
                    {
                        Denuncia = false,
                        Pergunta = new Pergunta { ID = pergunta.ID },
                        Resposta = null,
                        _Avaliacao = false,
                        Usuario = new Usuario { ID = UsuarioAtual.ID }
                    };
                    if (imgperguntalike.Source.ToString() == "File: like_ativo.png")
                    {
                        resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/byid/" + UsuarioAtual.ID + "/" + aval.Pergunta.ID);
                        aval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                        resposta = null;
                        aval._Avaliacao = false;
                        var valor = JsonConvert.SerializeObject(aval);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/alterar", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            imgperguntalike.Source = "like.png";
                            imgperguntadislike.Source = "dislike_ativo.png";
                        }
                    }
                    else
                    {
                        var valor = JsonConvert.SerializeObject(aval);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/inserir", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            imgperguntalike.Source = "like.png";
                            imgperguntadislike.Source = "dislike_ativo.png";
                        }
                    }
                };
                imgperguntadislike.GestureRecognizers.Add(tapPerguntaDislike);



                // Montando as respostas
                positivos = new Image[pergunta.Resposta.Count];
                negativos = new Image[pergunta.Resposta.Count];
                int index = 0;
                foreach (var item in pergunta.Resposta)
                {
                    resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/byid/" + UsuarioAtual.ID + "/" + item.ID);
                    Avaliacao verificador = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                    resposta = null;
                    if (verificador.ID == 0)
                    {
                        verificador.Denuncia = false;
                    }
                    if (item.Visibilidade == true && verificador.Denuncia == false)
                    {
                        resposta = await client.GetStringAsync(server.Servidor + "/api/Usuario/byid/" + item.Usuario);
                        usuario = JsonConvert.DeserializeObject<Usuario>(resposta);
                        resposta = null;
                        StackLayout infousuario = new StackLayout();
                        infousuario.Orientation = StackOrientation.Horizontal;
                        var img = new ImageCircle
                        {
                            WidthRequest = 100,
                            HeightRequest = 100,
                            Aspect = Aspect.AspectFill
                        };
                        if (usuario.Foto != null)
                        {
                            ms = new MemoryStream(usuario.Foto);
                            img.Source = ImageSource.FromStream(() => { return ms; });
                        }
                        else
                        {
                            img.Source = "user.png";
                        }
                        infousuario.Children.Add(img);
                        await Task.Delay(1000);
                        infousuario.Children.Add(new Label { Text = usuario.Nome, VerticalOptions = LayoutOptions.CenterAndExpand });
                        stlrespostas.Children.Add(infousuario);
                        stlrespostas.Children.Add(new Label { Text = item.Titulo, FontSize = 30 });
                        stlrespostas.Children.Add(new Label { Text = item.Texto, FontSize = 20 });
                        await Task.Delay(1000);
                        foreach (var it in item.Imagem)
                        {
                            Image image = new Image();
                            ms = new MemoryStream(it._Imagem);
                            image.Source = ImageSource.FromStream(() => { return ms; });
                            image.HorizontalOptions = LayoutOptions.CenterAndExpand;
                            stlrespostas.Children.Add(image);
                            await Task.Delay(1000);
                        }

                        // montando avaliacao do usuario


                        resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/byid/" + UsuarioAtual.ID + "/" + item.ID);
                        Avaliacao estaaval = new Avaliacao();
                        estaaval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                        resposta = null;
                        StackLayout avaliacao = new StackLayout();
                        Grid like = new Grid();
                        TapGestureRecognizer taplike = new TapGestureRecognizer();
                        taplike.Tapped += async (object sender, EventArgs e) =>
                        {
                            try
                            {
                                Label label = sender as Label;
                                Avaliacao aval = new Avaliacao
                                {
                                    Denuncia = false,
                                    Pergunta = null,
                                    Resposta = new Resposta { ID = Convert.ToInt32(label.Text.Trim().Substring(0, label.Text.IndexOf("-", StringComparison.Ordinal))) },
                                    _Avaliacao = true,
                                    Usuario = new Usuario { ID = UsuarioAtual.ID }
                                };
                                if (negativos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source.ToString() == "File: dislike_ativo.png")
                                {
                                    resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/byid/" + UsuarioAtual.ID + "/" + aval.Resposta.ID);
                                    aval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                                    resposta = null;
                                    aval._Avaliacao = true;
                                    var valor = JsonConvert.SerializeObject(aval);
                                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                    HttpResponseMessage rest = null;
                                    rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/alterar", conteudo);
                                    if (!rest.IsSuccessStatusCode)
                                    {
                                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                    }
                                    else
                                    {
                                        positivos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "like_ativo.png";
                                        negativos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "dislike.png";
                                    }
                                }
                                else
                                {
                                    var valor = JsonConvert.SerializeObject(aval);
                                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                    HttpResponseMessage rest = null;
                                    rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/inserir", conteudo);
                                    if (!rest.IsSuccessStatusCode)
                                    {
                                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                    }
                                    else
                                    {
                                        positivos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "like_ativo.png";
                                        negativos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "dislike.png";
                                    }
                                }
                            }
                            catch
                            {
                                await DisplayAlert("ERRO", "Erro de conexão", "OK");
                            }

                        };
                        Label lbllike = new Label
                        {
                            Text = item.ID.ToString() + "-" + index.ToString(),
                            WidthRequest = 40,
                            HeightRequest = 40,
                            FontSize = 0.001,
                            TextColor = Color.White,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.Transparent
                        };
                        lbllike.GestureRecognizers.Add(taplike);
                        Image imglike = new Image
                        {
                            Source = "like.png",
                            WidthRequest = 40,
                            HeightRequest = 40,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };
                        if (estaaval._Avaliacao == true && estaaval.ID != 0)
                        {
                            imglike.Source = "like_ativo.png";
                        }
                        positivos[index] = imglike;
                        like.Children.Add(imglike);
                        like.Children.Add(lbllike);
                        Grid dislike = new Grid();
                        TapGestureRecognizer tapdislike = new TapGestureRecognizer();
                        tapdislike.Tapped += async (object sender, EventArgs e) =>
                        {
                            try
                            {
                                Label label = sender as Label;
                                Avaliacao aval = new Avaliacao
                                {
                                    Denuncia = false,
                                    Pergunta = null,
                                    Resposta = new Resposta { ID = Convert.ToInt32(label.Text.Trim().Substring(0, label.Text.IndexOf("-", StringComparison.Ordinal))) },
                                    _Avaliacao = false,
                                    Usuario = new Usuario { ID = UsuarioAtual.ID }
                                };
                                if (positivos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source.ToString() == "File: like_ativo.png")
                                {
                                    resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/byid/" + UsuarioAtual.ID + "/" + aval.Resposta.ID);
                                    aval = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                                    resposta = null;
                                    aval._Avaliacao = false;
                                    var valor = JsonConvert.SerializeObject(aval);
                                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                    HttpResponseMessage rest = null;
                                    rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/alterar", conteudo);
                                    if (!rest.IsSuccessStatusCode)
                                    {
                                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                    }
                                    else
                                    {
                                        positivos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "like.png";
                                        negativos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "dislike_ativo.png";
                                    }
                                }
                                else
                                {
                                    var valor = JsonConvert.SerializeObject(aval);
                                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                    HttpResponseMessage rest = null;
                                    rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/inserir", conteudo);
                                    if (!rest.IsSuccessStatusCode)
                                    {
                                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                    }
                                    else
                                    {
                                        positivos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "like.png";
                                        negativos[Convert.ToInt32(label.Text.Trim().Replace(aval.Resposta.ID.ToString() + "-", ""))].Source = "dislike_ativo.png";
                                    }
                                }
                            }
                            catch
                            {
                                await DisplayAlert("ERRO", "Erro de conexão", "OK");
                            }

                        };
                        Label lbldislike = new Label
                        {
                            Text = item.ID.ToString() + "-" + index.ToString(),
                            WidthRequest = 40,
                            HeightRequest = 40,
                            FontSize = 0.001,
                            TextColor = Color.White,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.Transparent
                        };
                        lbldislike.GestureRecognizers.Add(tapdislike);
                        Image imgdislike = new Image
                        {
                            Source = "dislike.png",
                            WidthRequest = 40,
                            HeightRequest = 40,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };
                        if (estaaval._Avaliacao == false && estaaval.ID != 0)
                        {
                            imgdislike.Source = "dislike_ativo.png";
                        }
                        negativos[index] = imgdislike;
                        dislike.Children.Add(imgdislike);
                        dislike.Children.Add(lbldislike);
                        avaliacao.Orientation = StackOrientation.Horizontal;
                        avaliacao.Children.Add(like);
                        avaliacao.Children.Add(dislike);
                        Label denuncia = new Label
                        {
                            Text = "Denunciar",
                            TextColor = Color.FromHex("#000449"),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.Transparent
                        };
                        Label iddenuncia = new Label
                        {
                            Text = item.ID.ToString(),
                            FontSize = 0.0001,
                            BackgroundColor = Color.Transparent,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand

                        };
                        TapGestureRecognizer tapDenucia = new TapGestureRecognizer();
                        tapDenucia.Tapped += async (object sender, EventArgs e) =>
                        {
                            try
                            {
                                var confirmacao = await DisplayAlert("Denunciar ?", "Deseja denunciar essa resposta ? Essa ação não poderá ser desfeita", "Sim", "Não");
                                if (confirmacao == true)
                                {
                                    Label id = sender as Label;
                                    resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/resposta/byid/" + UsuarioAtual.ID + "/" + id.Text);
                                    Avaliacao esta = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                                    resposta = null;
                                    if (esta.ID == 0)
                                    {
                                        esta = new Avaliacao
                                        {
                                            Denuncia = true,
                                            Resposta = new Resposta { ID = Convert.ToInt32(id.Text) },
                                            Usuario = new Usuario { ID = UsuarioAtual.ID },
                                            _Avaliacao = false,
                                            Pergunta = null
                                        };
                                        var valor = JsonConvert.SerializeObject(esta);
                                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                        HttpResponseMessage rest = null;
                                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/inserir", conteudo);
                                        if (!rest.IsSuccessStatusCode)
                                        {
                                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                        }
                                        else
                                        {
                                            frm_carregando.IsVisible = true;
                                            await CarregarPerguntaAsync();
                                            frm_carregando.IsVisible = false;
                                        }
                                    }
                                    else
                                    {
                                        esta.Denuncia = true;
                                        var valor = JsonConvert.SerializeObject(esta);
                                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                                        HttpResponseMessage rest = null;
                                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/resposta/alterar", conteudo);
                                        if (!rest.IsSuccessStatusCode)
                                        {
                                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                                        }
                                        else
                                        {
                                            frm_carregando.IsVisible = true;
                                            await CarregarPerguntaAsync();
                                            frm_carregando.IsVisible = false;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                await DisplayAlert("ERRO", "Erro de conexão", "OK");
                            }

                        };
                        iddenuncia.GestureRecognizers.Add(tapDenucia);


                        Grid gridDenuncia = new Grid();
                        gridDenuncia.Children.Add(denuncia);
                        gridDenuncia.Children.Add(iddenuncia);


                        avaliacao.Children.Add(gridDenuncia);
                        stlrespostas.Children.Add(avaliacao);
                        stlrespostas.Children.Add(new Label { Text = "---------------------------------------" });
                        index += 1;
                    }
                }

            }
            catch 
            {
                await DisplayAlert("ERRO", "Erro de conexão", "OK");
                await Navigation.PopAsync();
            }
           
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            frm_carregando.IsVisible = true;
            await CarregarPerguntaAsync();
            frm_carregando.IsVisible = false;

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new AddRespostaPage(pergunta.ID));
        }

        async void Denuncia_ClickedAsync(object sender, System.EventArgs e)
        {
            try
            {
                var confirmacao = await DisplayAlert("Denunciar ?", "Deseja denunciar essa pergunta ? Essa ação não poderá ser desfeita", "Sim", "Não");
                if (confirmacao)
                {
                    var resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/byid/" + UsuarioAtual.ID + "/" + pergunta.ID);
                    Avaliacao esta = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                    if (esta.ID == 0)
                    {
                        esta = new Avaliacao
                        {
                            Denuncia = true,
                            Resposta = null,
                            Usuario = new Usuario { ID = UsuarioAtual.ID },
                            _Avaliacao = false,
                            Pergunta = new Pergunta { ID = pergunta.ID }
                        };
                        var valor = JsonConvert.SerializeObject(esta);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/inserir", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        esta.Denuncia = true;
                        var valor = JsonConvert.SerializeObject(esta);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage rest = null;
                        rest = await client.PostAsync(server.Servidor + "/api/Avaliacao/pergunta/alterar", conteudo);
                        if (!rest.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                        }
                        else
                        {
                            await Navigation.PopAsync();
                        }
                    }
                }

            }
            catch 
            {
                await DisplayAlert("ERRO", "Erro de conexão", "OK");
            }
        }
    }
}
