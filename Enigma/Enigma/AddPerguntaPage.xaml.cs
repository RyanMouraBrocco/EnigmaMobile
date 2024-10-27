using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using EnigmaClass;
using Newtonsoft.Json;
using Plugin.Media;
using Xamarin.Forms;

namespace Enigma
{
    public partial class AddPerguntaPage : ContentPage
    {
        bool processar = true;
        HttpClient client = new HttpClient();
        Server server = new Server();
        bool isbusy = false;
        List<Image> Imagens = new List<Image>();
        List<Imagem> Imgs = new List<Imagem>();
        byte[] bytes;

        public AddPerguntaPage()
        {
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (object sender, EventArgs e) =>
            {
                Navigation.PopModalAsync();
            };
            BtnSair.GestureRecognizers.Add(tap);

        }

         async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                txttitulo.IsEnabled = false;
                txtpergunta.IsEnabled = false;
                isbusy = true;
                frm_carregando.IsVisible = true;
              try
              {
                    if (txttitulo.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Preencha o Título", "OK");
                        processar = false;
                    }
                    if (txtpergunta.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Escreva uma pergunta", "OK");
                        processar = false;
                    }
                    if (processar)
                    {
                       try
                       {
                            Pergunta pergunta = new Pergunta
                            {
                                Imagem = new List<Imagem>(),
                                Resposta = new List<Resposta>(),
                                Texto = txtpergunta.Text.Trim(),
                                Usuario = UsuarioAtual.ID,
                                Visibilidade = true,
                                Titulo = txttitulo.Text.Trim()
                            };
                            if (Imgs.Count>0)
                            {
                                foreach (var item in Imgs)
                                {
                                    pergunta.Imagem.Add(item);
                                }

                            }
                            var valor = JsonConvert.SerializeObject(pergunta);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resposta = null;
                            resposta = await client.PostAsync(server.Servidor + "/api/Pergunta/inserir", conteudo);
                            if (!resposta.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                            }
                            else
                            {
                                await Navigation.PopModalAsync();
                            }

                      }
                      catch
                      {
                            await DisplayAlert("Erro", "Falta de Conexão", "OK");
                      }
                    }
                    processar = true;
                    txttitulo.IsEnabled = true;
                    txtpergunta.IsEnabled = true;
                    frm_carregando.IsVisible = false;
                    isbusy = false;

                }
                catch
                {
                    processar = true;
                    txttitulo.IsEnabled = true;
                    txtpergunta.IsEnabled = true;
                    frm_carregando.IsVisible = false;
                    isbusy = false;
                    await DisplayAlert("Erro", "Preencha os Campos", "OK");
                }
            }
             
        }

        async void Tirar_ClickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy == false)
            {
                isbusy = true;
                try
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                    {
                        await DisplayAlert("Erro", "Nenhuma câmera detectada.", "OK");
                        isbusy = false;
                        return;
                    }

                    var file = await CrossMedia.Current.TakePhotoAsync(
                    new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        SaveToAlbum = true,
                        Directory = "Enigma"
                    });

                    if (file == null)
                    {
                        isbusy = false;
                        return;
                    }
                    var arquivo = file.GetStream();
                    MemoryStream memoryStream = new MemoryStream();
                    arquivo.CopyTo(memoryStream);
                    Image imgfoto = new Image();
                    imgfoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;

                    });
                    bytes = memoryStream.ToArray();
                    Imagem imagem = new Imagem 
                    { 
                        Extensao= ".jpg",
                        Nome="Perg",
                        Usuario = UsuarioAtual.ID,
                        _Imagem = bytes,
                        ID = 0
                    };
                    Imgs.Add(imagem);
                    Imagens.Add(imgfoto);
                    CarregarList();
                    isbusy = false;
                }
                catch
                {
                    isbusy = false;
                    await DisplayAlert("Erro", "Erro ao acessar a câmera. Vá em configurações e permita o acesso", "OK");
                }

            }



        }

        async void Escolher_ClickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy == false)
            {
                isbusy = true;
                try
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("ERRO", "Galeria de fotos não suportada.", "OK");
                        isbusy = false;
                        return;
                    }

                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if (file == null)
                    {
                        isbusy = false;
                        return;
                    }
                    var arquivo = file.GetStream();
                    MemoryStream memoryStream = new MemoryStream();
                    arquivo.CopyTo(memoryStream);
                    Image imgfoto = new Image();
                    imgfoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;

                    });
                    bytes = memoryStream.ToArray();
                    Imagem imagem = new Imagem
                    {
                        Extensao = ".jpg",
                        Nome = "Perg",
                        Usuario = UsuarioAtual.ID,
                        _Imagem = bytes,
                        ID = 0
                    };
                    Imgs.Add(imagem);
                    Imagens.Add(imgfoto);
                    CarregarList();
                    isbusy = false;
                }
                catch
                {
                    isbusy = false;
                    await DisplayAlert("Erro", "Erro ao acessar a câmera. Vá em configurações e permita o acesso", "OK");
                }
            }
        }
        public void CarregarList()
        {
            list.ItemsSource = null;
            list.ItemsSource = Imagens;
        }
    }
}
