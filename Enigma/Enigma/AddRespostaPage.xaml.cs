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
    public partial class AddRespostaPage : ContentPage
    {
        bool processar = true;
        HttpClient client = new HttpClient();
        Server server = new Server();
        Pergunta pergunta = new Pergunta();
        bool isbusy = false;
        List<Image> Imagens = new List<Image>();
        List<Imagem> Imgs = new List<Imagem>();
        byte[] bytes;

        public AddRespostaPage(int idpergunta)
        {
            InitializeComponent();
            pergunta.ID = idpergunta;
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
                txtresposta.IsEnabled = false;
                isbusy = true;
                frm_carregando.IsVisible = true;
                try
                {
                    if (txttitulo.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Preencha o Título", "OK");
                        processar = false;
                    }
                    if (txtresposta.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Escreva uma resposta", "OK");
                        processar = false;
                    }
                    if (processar)
                    {
                        try
                        {
                            Resposta respostaperg = new Resposta
                            {
                                Imagem = new List<Imagem>(),
                                Pergunta = pergunta,
                                Texto = txtresposta.Text.Trim(),
                                Usuario = UsuarioAtual.ID,
                                Visibilidade = true,
                                Titulo = txttitulo.Text.Trim()
                            };
                            if (Imgs.Count > 0)
                            {
                                foreach (var item in Imgs)
                                {
                                    respostaperg.Imagem.Add(item);
                                }

                            }
                            var valor = JsonConvert.SerializeObject(respostaperg);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resposta = null;
                            resposta = await client.PostAsync(server.Servidor + "/api/Resposta/inserir", conteudo);
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
                    isbusy = false;
                    txttitulo.IsEnabled = true;
                    txtresposta.IsEnabled = true;
                    frm_carregando.IsVisible = false;
                    processar = true;
                }
                catch
                {
                    isbusy = false;
                    txttitulo.IsEnabled = true;
                    txtresposta.IsEnabled = true;
                    frm_carregando.IsVisible = false;
                    processar = true;
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
                        Extensao = ".jpg",
                        Nome = "Resp",
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
                        Nome = "Resp",
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
