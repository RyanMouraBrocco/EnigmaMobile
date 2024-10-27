using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace Enigma
{
    public partial class ConfigPage : ContentPage
    {
        bool carregar = false;
        public ConfigPage()
        {
            InitializeComponent();
            CarregarPerfil();
        }

        public void CarregarPerfil()
        {
            Imagem.Children.Clear();
            var photo = new ImageCircle
            {
                WidthRequest = 100,
                HeightRequest = 100,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center
            };
            if (UsuarioAtual.Foto != null)
            {
                MemoryStream ms = new MemoryStream(UsuarioAtual.Foto);
                photo.Source = ImageSource.FromStream(() => { return ms; });
            }
            else
            {
                photo.Source = "user.png";
            }
            Imagem.Children.Add(photo);
            LblNome.Text = UsuarioAtual.Nome; 
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (carregar)
            {
                CarregarPerfil();
                carregar = false;
            }
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                UsuarioAtual.Email = string.Empty;
                UsuarioAtual.Nome = string.Empty;
                UsuarioAtual.Foto = null;
                UsuarioAtual.TipoConta = string.Empty;
                UsuarioAtual.ID = 0;
                UsuarioAtual.Senha = string.Empty;
                Navigation.PopModalAsync();
            }
            catch 
            {

            }
        }
        void Perfil_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                Navigation.PushModalAsync(new PerfilPage());
                carregar = true;
            }
            catch
            {

            }
        }

        void Minha_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new MinhasPerguntasERespostasPage());
        }

        void Desempenho_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new DesempenhoPage());
        }
    }
}
