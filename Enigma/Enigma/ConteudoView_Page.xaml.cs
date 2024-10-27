using System;
using System.Collections.Generic;
using System.IO;
using EnigmaClass;
using Xamarin.Forms;
using Octane.Xamarin.Forms.VideoPlayer;

namespace Enigma
{
    public partial class ConteudoView_Page : ContentPage
    {
        int idconteudo;
        public ConteudoView_Page(Conteudo conteudo)
        {
            InitializeComponent();
            idconteudo = conteudo.ID;
            Title = conteudo.Nome;  
            foreach (var item in conteudo.ConteudoTexto)
            {
                if (item.Texto!=null)
                {
                    Label label = new Label();
                    label.Text = item.Texto.Conteudo;
                    label.FontSize = (double)item.Texto.Tamanho;
                    if (item.Texto.Italico==true)
                    {
                        label.FontAttributes = FontAttributes.Italic;
                    }
                    if (item.Texto.Negrito == true)
                    {
                        label.FontAttributes = FontAttributes.Bold;
                    }
                    label.TextColor = Color.FromHex(item.Texto.Cor);
                    stl.Children.Add(label);
                }
                if (item.Imagem != null)
                {
                    Image image = new Image();
                    MemoryStream ms = new MemoryStream(item.Imagem._Imagem);
                    image.Source=ImageSource.FromStream(()=> { return ms; });
                    image.HorizontalOptions = LayoutOptions.FillAndExpand;
                    image.VerticalOptions = LayoutOptions.FillAndExpand;
                    image.Aspect = Aspect.Fill;
                    stl.Children.Add(image);
                }
                if (item.Video != null)
                {
                    StackLayout stack = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    VideoPlayer player = new VideoPlayer
                    {
                        AutoPlay = false,
                        Source = item.Video.Link,
                        DisplayControls = true
                    };
                    Label label = new Label
                    {
                        Text = item.Video.Nome,
                        TextColor = Color.FromHex("000449"),
                        FontSize = 20.0,
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    };
                    stack.Children.Add(label);
                    stack.Children.Add(player);
                    stl.Children.Add(stack);
                }
            }

        }

        void Exercicio_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ExerciciosPage(idconteudo));
        }
    }
}
