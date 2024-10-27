using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using EnigmaClass;
using Newtonsoft.Json;
using System.Text;

namespace Enigma
{
    public partial class CadastroPage : ContentPage
    {
        bool processar = true;
        HttpClient client = new HttpClient();
        Server server = new Server();
        public CadastroPage()
        {
            InitializeComponent();
        }
        async void Cadastrar_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                if (TxtNome.Text.Trim() == string.Empty)
                {
                    await DisplayAlert("Erro", "Preencha o nome", "OK");
                    processar = false;
                }
                if (TxtEmail.Text.Trim() == string.Empty)
                {
                    await DisplayAlert("Erro", "Preencha o email", "OK");
                    processar = false;
                }
                else
                {
                    if (!TxtEmail.Text.Trim().Contains("@") || !TxtEmail.Text.Trim().Contains(".com"))
                    {
                        await DisplayAlert("Erro", "Email inválido", "OK");
                        processar = false;
                    }
                    else
                    {
                        try
                        {
                            var resposta = await client.GetStringAsync(server.Servidor+"/api/Usuario/bylogin?login=" + TxtEmail.Text.Trim());
                            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(resposta);
                            if (usuario.ID != 0)
                            {
                                await DisplayAlert("Erro", "Email já cadastrado", "OK");
                                processar = false;
                            }
                        }
                        catch
                        {
                            await DisplayAlert("Erro", "Falta de conexão tente mais tarde", "OK");
                            processar = false;
                        }
                    }
                }
                if (TxtSenha.Text.Trim() == string.Empty)
                {
                    await DisplayAlert("Erro", "Preencha a senha", "OK");
                    processar = false;
                }
                else
                {
                    if (TxtSenha.Text.Trim().Contains("@") || TxtSenha.Text.Trim().Contains(" ") || TxtSenha.Text.Trim().Contains("-") || TxtSenha.Text.Trim().Contains("+") || TxtSenha.Text.Trim().Contains("˜") || TxtSenha.Text.Trim().Contains(",") || TxtSenha.Text.Trim().Contains("?"))
                    {
                        await DisplayAlert("Erro", "A senha somente deve conter números e/ou letras", "OK");
                        processar = false;
                    }
                    else
                    {
                        if (TxtSenha.Text.Trim().Length != 8)
                        {
                            await DisplayAlert("Erro", "A senha deve conter 8 digitos", "OK");
                            processar = false;
                        }
                        else
                        {
                            if (TxtSenha.Text.Trim() != TxtConfirmarSenha.Text.Trim())
                            {
                                await DisplayAlert("Erro", "As senhas não combinam", "OK");
                                processar = false;
                            }
                        }
                    }
                }
                if (processar)
                {
                    try
                    {
                        Usuario usuario = new Usuario
                        {
                            ID = 0,
                            Nome = TxtNome.Text.Trim(),
                            Email = TxtEmail.Text.Trim().ToLower(),
                            Foto = null,
                            Senha = TxtSenha.Text.Trim(),
                            TipoConta = "E"
                        };
                        var valor = JsonConvert.SerializeObject(usuario);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage resposta = null;
                        resposta = await client.PostAsync(server.Servidor + "/api/Usuario/inserir", conteudo);
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
                        await DisplayAlert("Erro", "Falta de conexão tente mais tarde", "OK");
                    }
                }
                processar = true;
            }
            catch
            {
                await DisplayAlert("Erro", "Preencha os campos", "OK");
            }

        }

        void Cancelar_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();

        }

    }
}
