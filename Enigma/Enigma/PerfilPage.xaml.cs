using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using System.Drawing;
using System.Text;

namespace Enigma
{
    public partial class PerfilPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        bool processar = true;
        byte[] Foto = null;
        bool isbusy = false;

        public PerfilPage()
        {
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped+= (object sender, EventArgs e) => 
            {
                Navigation.PopModalAsync();
            };
            btnsair.GestureRecognizers.Add(tap);
            txtnome.Text = UsuarioAtual.Nome;
            txtemail.Text = UsuarioAtual.Email;
            if (UsuarioAtual.Foto!=null)
            {
                MemoryStream memoryStream = new MemoryStream(UsuarioAtual.Foto);
                imgfoto.Source = ImageSource.FromStream(() => { return memoryStream; });
            }
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            try
            {
                if (isbusy == false)
                {
                    txtnome.IsEnabled = false;
                    txtemail.IsEnabled = false;
                    frm_carregando.IsVisible=true;
                    isbusy = true;
                    if (txtnome.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Digite o nome", "OK");
                        processar = false;
                    }
                    if (txtemail.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Digite o email", "OK");
                        processar = false;
                    }
                    else
                    {
                        if (txtemail.Text.Trim() != UsuarioAtual.Email)
                        {
                            var resposta = await client.GetStringAsync(server.Servidor + "/api/Usuario/bylogin?login=" + txtemail.Text.Trim());
                            Usuario u = JsonConvert.DeserializeObject<Usuario>(resposta);
                            if (u.ID != 0)
                            {
                                await DisplayAlert("Erro", "Email já cadastrado, digite outro", "OK");
                                processar = false;
                            }
                        }
                        else
                        {
                            if (!txtemail.Text.Trim().Contains("@") || !txtemail.Text.Trim().Contains(".com"))
                            {
                                await DisplayAlert("Erro", "Email inválido", "OK");
                                processar = false;
                            }
                        }
                    }
                    if (processar)
                    {
                        Usuario usuario = new Usuario
                        {
                            ID = UsuarioAtual.ID,
                            Email = txtemail.Text.Trim().ToLower(),
                            Nome = txtnome.Text.Trim(),
                            Senha = string.Empty,
                            TipoConta = UsuarioAtual.TipoConta
                        };
                        if (Foto != null)
                        {
                            usuario.Foto = Foto;
                        }
                        else
                        {
                            usuario.Foto = UsuarioAtual.Foto;
                        }
                        try
                        {
                            var valor = JsonConvert.SerializeObject(usuario);
                            var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                            HttpResponseMessage resposta = null;
                            resposta = await client.PostAsync(server.Servidor + "/api/Usuario/alterar", conteudo);
                            if (!resposta.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                            }
                            else
                            {
                                UsuarioAtual.Nome = txtnome.Text.Trim();
                                UsuarioAtual.Email = txtemail.Text.Trim();
                                if (Foto != null)
                                {
                                    UsuarioAtual.Foto = Foto;
                                }
                                isbusy = false;
                                await Navigation.PopModalAsync();
                            }
                        }
                        catch 
                        {
                            isbusy = false;
                            await DisplayAlert("Erro", "Erro de conexão", "OK");
                        }
                    }
                    txtnome.IsEnabled = true;
                    txtemail.IsEnabled = true;
                    isbusy = false;
                    frm_carregando.IsVisible = false;
                    processar = true;
                }
            }
            catch 
            {
                txtnome.IsEnabled = true;
                txtemail.IsEnabled = true;
                frm_carregando.IsVisible = false;
                processar = true;
                isbusy = false;
                await DisplayAlert("Erro", "Preencha todos os campos", "OK");
            }

        }

        async void Tirar_ClickedAsync(object sender, System.EventArgs e)
        {
            if (isbusy==false)
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
                    imgfoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;

                    });
                    Foto = memoryStream.ToArray();
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
            if (isbusy==false)
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
                    imgfoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;

                    });
                    Foto = memoryStream.ToArray();
                    isbusy = false;
                }
                catch
                {
                    isbusy = false;
                    await DisplayAlert("Erro", "Erro ao acessar a câmera. Vá em configurações e permita o acesso", "OK");
                }
            }


        }
    }
}
