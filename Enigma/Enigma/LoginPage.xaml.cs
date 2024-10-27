using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;

using Xamarin.Forms;

namespace Enigma
{
    public partial class LoginPage : ContentPage
    {
        bool processar = true;
        HttpClient client = new HttpClient();
        Server server = new Server();
        bool isbusy = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (isbusy == false)
            {
                frm_carregando.IsVisible = true;
                TxtLogin.IsEnabled = false;
                TxtSenha.IsEnabled = false;
                isbusy = true;
                try
                {
                    if (TxtLogin.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Digite o Login", "OK");
                        processar = false;
                    }
                    if (TxtSenha.Text.Trim() == "")
                    {
                        await DisplayAlert("Erro", "Digite o Senha", "OK");
                        processar = false;
                    }
                    if (processar)
                    {
                        try
                        {
                            var resposta = await client.GetStringAsync(server.Servidor + "/api/Usuario/logar/" + TxtLogin.Text.Trim() + "/" + TxtSenha.Text.Trim());
                            bool logado = JsonConvert.DeserializeObject<bool>(resposta);
                            if (logado)
                            {

                                Usuario atual = new Usuario();
                                resposta = await client.GetStringAsync(server.Servidor + "/api/Usuario/bylogin?Login=" + TxtLogin.Text.Trim());
                                atual = JsonConvert.DeserializeObject<Usuario>(resposta);
                                if (atual.TipoConta.Trim() != "B")
                                {
                                    UsuarioAtual.ID = atual.ID;
                                    UsuarioAtual.Nome = atual.Nome;
                                    UsuarioAtual.Email = atual.Email;
                                    UsuarioAtual.TipoConta = atual.TipoConta;
                                    UsuarioAtual.Foto = atual.Foto;
                                    frm_carregando.IsVisible = false;
                                    isbusy = false;
                                    await Navigation.PushModalAsync(new MasterDetalhesPage());
                                    TxtLogin.IsEnabled = true;
                                    TxtSenha.IsEnabled = true;
                                    TxtLogin.Text = "";
                                    TxtSenha.Text = "";
                                }
                                else
                                {
                                    await DisplayAlert("Erro", "Conta Banida", "OK");
                                }

                            }
                            else
                            {
                                await DisplayAlert("Erro", "Login ou senha incorretos", "OK");
                            }
                        }
                        catch
                        {
                            await DisplayAlert("Erro", "Erro na conexão", "OK");
                        }

                    }
                    isbusy = false;
                    TxtLogin.IsEnabled = true;
                    TxtSenha.IsEnabled = true;
                    frm_carregando.IsVisible = false;
                    processar = true;
                }
                catch
                {
                    frm_carregando.IsVisible = false;
                    TxtLogin.IsEnabled = true;
                    TxtSenha.IsEnabled = true;
                    processar = true;
                    await DisplayAlert("Erro", "Preencha os Campos", "OK");
                    isbusy = false;
                }
            }
        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                Navigation.PushModalAsync(new CadastroPage());
                isbusy = false;
            }

        }
    }
}
