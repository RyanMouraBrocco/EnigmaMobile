using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class ForumPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        List<Pergunta> Perguntas = new List<Pergunta>();
        bool isbusy = false;
        public ForumPage()
        {
            InitializeComponent();
        }
        async System.Threading.Tasks.Task BuscarAsync()
        {
            frm_carregando.IsVisible = true;
            Lista.ItemsSource = null;
            try
            {
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Pergunta/all");
                Perguntas = JsonConvert.DeserializeObject<List<Pergunta>>(resposta);
                foreach (var item in Perguntas.Where(x => x.Visibilidade == true))
                {
                    if (item.Visibilidade==true)
                    {
                        resposta = await client.GetStringAsync(server.Servidor + "/api/Avaliacao/pergunta/byid/" + UsuarioAtual.ID + "/" + item.ID);
                        Avaliacao avaliacao = JsonConvert.DeserializeObject<Avaliacao>(resposta);
                        if (avaliacao.Denuncia == true)
                        {
                            item.Visibilidade = false;
                        }
                    }

                }
                Lista.ItemsSource = Perguntas.Where(x=>x.Visibilidade==true);
                frm_carregando.IsVisible = false;
            }
            catch 
            {
                frm_carregando.IsVisible = false;
                await DisplayAlert("ERRO", "Falta de Conexão", "OK");
            }
        }

        async void Handle_RefreshingAsync(object sender, System.EventArgs e)
        {
            if (txtbusca.Text==null)
            {
                await BuscarAsync();
            }
            Lista.IsRefreshing = false;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await BuscarAsync();

        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (txtbusca.Text!=null)
            {
                Lista.ItemsSource = null;
                try
                {
                    Lista.ItemsSource = Perguntas.Where(x => x.Titulo.ToUpper().Contains(txtbusca.Text.Trim().ToUpper()) && x.Visibilidade == true);
                }
                catch 
                {
                    txtbusca.Text = "";
                }
            }
            else
            {
                Lista.ItemsSource = null;
                Lista.ItemsSource = Perguntas;
            }
        }

        void Nova_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new AddPerguntaPage());
        }

        async void Handle_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isbusy==false)
            {
                try
                {
                    isbusy = true;
                    frm_carregando.IsVisible = true;
                    Pergunta pergunta = new Pergunta();
                    pergunta = (sender as ListView).SelectedItem as Pergunta;
                    //var resposta = await client.GetStringAsync(server.Servidor + "/api/Pergunta/byid/" + pergunta.ID);
                    //pergunta = JsonConvert.DeserializeObject<Pergunta>(resposta);
                    frm_carregando.IsVisible = false;
                    isbusy = false;
                    await Navigation.PushAsync(new PerguntaViewPage(pergunta));
                }
                catch
                {
                    isbusy = false;
                    frm_carregando.IsVisible = false;
                    await DisplayAlert("Erro", "Erro de Conexão", "OK");
                }
            }
           
        }

    }
}
