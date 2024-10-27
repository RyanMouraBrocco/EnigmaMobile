using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;
namespace Enigma
{
    public partial class MateriasPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        List<Materia> materias = new List<Materia>();
        ColumnDefinition c1 = new ColumnDefinition();
        ColumnDefinition c2 = new ColumnDefinition();
        bool pri = true;
        int l = 0, c = 0;
        bool isbusy = false;

        public MateriasPage()
        {
            InitializeComponent();
            c1.Width = 150;
            c2.Width = 150;
            _grid.ColumnDefinitions.Add(c1);
            _grid.ColumnDefinitions.Add(c2);
        }
        public async System.Threading.Tasks.Task CarregarAsync()
        {
            try
            {
                _grid.RowDefinitions.Clear();
                _grid.ColumnDefinitions.Clear();
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Materia/all");
                materias = JsonConvert.DeserializeObject<List<Materia>>(resposta);
                bool criar = true;
                foreach (var item in materias)
                {
                    if (criar == true)
                    {
                        RowDefinition d = new RowDefinition();
                        d.Height = 100;
                        _grid.RowDefinitions.Add(d);
                        criar = false;
                    }
                    else
                    {
                        criar = true;
                    }
                    var botao = new Button
                    {
                        Text = item.Nome,
                        BackgroundColor = Color.Transparent,
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    botao.Clicked += async (sender, e) =>
                    {
                        if (isbusy==false)
                        {
                            isbusy = true;
                            Button a = sender as Button;
                            try
                            {
                                frm_carregando.IsVisible = true;
                                var r = await client.GetStringAsync(server.Servidor + "/api/Materia/bynome/" + a.Text);
                                Materia m = JsonConvert.DeserializeObject<Materia>(r);
                                frm_carregando.IsVisible = false;
                                await Navigation.PushAsync(new Conteudos_Page(m));
                                isbusy = false;
                            }
                            catch
                            {
                                frm_carregando.IsVisible = false;
                                isbusy = false;
                                await DisplayAlert("ERRO", "Sem conexão", "OK");
                            }
                        }

                    };
                    if (item.Imagem != null)
                    {
                        MemoryStream ms = new MemoryStream(item.Imagem);
                        Image img = new Image();
                        img.Source = ImageSource.FromStream(() => { return ms; });
                        img.HorizontalOptions = LayoutOptions.FillAndExpand;
                        img.VerticalOptions = LayoutOptions.FillAndExpand;
                        _grid.Children.Add(img, c, l);
                    }
                    else
                    {
                        botao.BackgroundColor = Color.FromHex("#000449");
                    }
                    _grid.Children.Add(botao, c, l);
                    if (c == 0)
                    {
                        c = 1;
                    }
                    else
                    {
                        c = 0;
                        l += 1;
                    }
                }
            }
            catch 
            {
                await DisplayAlert("ERRO", "Sem conexão", "OK");
            }
           

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (pri == true)
            {
                frm_carregando.IsVisible = true;
                await CarregarAsync();
                frm_carregando.IsVisible = false;
                pri = false;
            }
            else
            {
                if (Controle.verificar)
                {
                    try
                    {
                        TimeSpan timeSpan = DateTime.Now - Controle.data;
                        decimal horas = (decimal)timeSpan.Hours;
                        decimal minutos = 0;
                        if (timeSpan.Minutes>0)
                        {
                            minutos = (decimal)timeSpan.Minutes / (decimal)60;
                        }
                        if (timeSpan.Days>0)
                        {
                            horas += 24 * timeSpan.Days;
                        }
                        Controle.desempenho.HorasEstudadas = horas+minutos;
                        var valor = JsonConvert.SerializeObject(Controle.desempenho);
                        var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                        HttpResponseMessage resp = null;
                        resp = await client.PostAsync(server.Servidor + "/api/Desempenho/atualizarhoras", conteudo);
                    }
                    catch
                    {

                    }
                    Controle.verificar = false;
                }
            }
        }
    }
}
