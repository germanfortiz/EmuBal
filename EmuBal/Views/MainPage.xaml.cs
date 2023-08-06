using EmuBal.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using Windows.Media.AppBroadcasting;

namespace EmuBal.Views;

// SALIDA:  INDICADOR LE 100 1  (disponible en el conector tipo AMP DB9 macho)
// PIN3         =      TXD
// PIN5         =      GND
// ENTRADA: EN CPU  (PUERTO SERIE)
//                 CON PUERTO DE 9 PINES
//                 PIN 2           =          RXD
//                 PIN 5           =          GND
//                 Además se deben puentear pines 4 con 6 y 7 con 8
//                 OPCIONAL (CUANDO HAY UN PUERTO DE 25 PINES)
//                 PIN 3           =          RXD
//                 PIN 7           =          GND
//                 Además se deben puentear pines 4 con 5 y 6 con 20
// Los equipos se entregan programados en forma standard con los siguientes parámetros de transmisión: 
// 1200 BAUD, 8 BIT, SIN PARIDAD, UN BIT DE STOP.
// Se envía constantemente un string de ocho caracteres con el siguiente formato:           
//                                                                   “S P P P P P P CR” 
// SIENDO:  S = BYTE DE STATUS;      P = PESO EN ASC II;        CR = RETORNO CARRO (Odh)
// BYTE DE ESTATUS EN BINARIO        BYTE DE ESTATUS EN ASC II
// BIT 0:   NETO                     @..C   : PESO POSITIVO FUERA DE EQUILIBRIO
// BIT 1:   CENTRO DE CERO           D      : PESO BRUTO MAYOR QUE CERO
// BIT 2:   EQUILIBRIO               E      : PESO NETO MAYOR QUE CERO
// BIT 3:   PESO NEGATIVO            F      : CENTRO DE CERO SIN TARA
// BIT 4:   FUERA DE RANGO           G      : CENTRO DE CERO CON TARA
// BIT 5:   0                        H..K   : PESO NEGATIVO FUERA DE EQUILIBRIO
// BIT 6:   1                        L..O   : PESO NEGATIVO EN EQUILIBRIO
// BIT 7:   0                        P...   : FUERA DE RANGO
// LIS

public sealed partial class MainPage : Page
{
    private DispatcherTimer dTimer;
    private static SerialPort SerialPort = null;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        this.Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        dTimer = new DispatcherTimer();
        dTimer.Tick += DTimer_Tick;

        this.cbPorts.Items.Clear();

        if (SerialPort.GetPortNames().Count() > 0)
        {
            this.cbPorts.PlaceholderText = "Seleccionar puerto serie";
            foreach (var portName in SerialPort.GetPortNames())
            {
                this.cbPorts.Items.Add(portName);
            }
        }
        else
        {
            this.cbPorts.PlaceholderText = "Che, no hay ningún puerto serie";
        }
    }

    private void cbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SerialPort == null)
        {
            SerialPort = new SerialPort();
        }

        if (SerialPort.IsOpen)
        {
            SerialPort.Close();
        }

        // Allow the user to set the appropriate properties.
        SerialPort.PortName = cbPorts.Items[cbPorts.SelectedIndex].ToString();
        SerialPort.BaudRate = 1200;
        SerialPort.Parity = Parity.None;
        SerialPort.DataBits = 8;
        SerialPort.StopBits = StopBits.One;
        SerialPort.Handshake = Handshake.None;

        // Set the read/write timeouts
        SerialPort.ReadTimeout = 500;
        SerialPort.WriteTimeout = 500;

        try
        {
            SerialPort.Open();
        }
        catch (Exception)
        { 
            SerialPort?.Close();
        }
    }

    private void sSampling_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (dTimer == null)
            return;
        if (tOutput == null)
            return;

        if (dTimer.IsEnabled)
            dTimer.Stop();
        dTimer.Interval = TimeSpan.FromSeconds(1 / sSampling.Value);
        tOutput.Text = string.Empty;
        dTimer.Start();
    }

    private void rbAscii_Checked(object sender, RoutedEventArgs e)
    {
        if (bAsciiFormat != null) bAsciiFormat.Visibility = Visibility.Visible;
        if (bBinaryFormat != null) bBinaryFormat.Visibility = Visibility.Collapsed;
    }

    private void rbBinary_Checked(object sender, RoutedEventArgs e)
    {
        if (bAsciiFormat != null) bAsciiFormat.Visibility = Visibility.Collapsed;
        if (bBinaryFormat != null) bBinaryFormat.Visibility = Visibility.Visible;
    }

    private void DTimer_Tick(object? sender, object e)
    {
        if ((SerialPort == null) || (!SerialPort.IsOpen))
        {
            tOutput.Text = "Puerto Serie no seleccionado o no lo estamos pudiendo abrir.";
            return;
        }

        byte[] commandToSend = new byte[8];
        string textToShow = string.Empty;

        if (rbAscii.IsChecked == true)
        {
            textToShow = "@";
            if (rbStatusA.IsChecked == true)
            {
                textToShow = "A";
            }
            else
            if (rbStatusB.IsChecked == true)
            {
                textToShow = "B";
            }
            else
            if (rbStatusC.IsChecked == true)
            {
                textToShow = "C";
            }
            else
            if (rbStatusD.IsChecked == true)
            {
                textToShow = "D";
            }
            else
            if (rbStatusE.IsChecked == true)
            {
                textToShow = "E";
            }
            else
            if (rbStatusF.IsChecked == true)
            {
                textToShow = "F";
            }
            else
            if (rbStatusG.IsChecked == true)
            {
                textToShow = "G";
            }
            else
            if (rbStatusH.IsChecked == true)
            {
                textToShow = "H";
            }
            else
            if (rbStatusI.IsChecked == true)
            {
                textToShow = "I";
            }
            else
            if (rbStatusJ.IsChecked == true)
            {
                textToShow = "J";
            }
            else
            if (rbStatusK.IsChecked == true)
            {
                textToShow = "K";
            }
            else
            if (rbStatusL.IsChecked == true)
            {
                textToShow = "L";
            }
            else
            if (rbStatusM.IsChecked == true)
            {
                textToShow = "M";
            }
            else
            if (rbStatusN.IsChecked == true)
            {
                textToShow = "N";
            }
            else
            if (rbStatusO.IsChecked == true)
            {
                textToShow = "O";
            }
            else
            if (rbStatusP.IsChecked == true)
            {
                textToShow = "P";
            }
            else
            if (rbStatusQ.IsChecked == true)
            {
                textToShow = "Q";
            }
            else
            if (rbStatusR.IsChecked == true)
            {
                textToShow = "R";
            }
            else
            if (rbStatusS.IsChecked == true)
            {
                textToShow = "S";
            }

            textToShow += string.Format("{0,6:D6}", (int)sPeso.Value);

            for (var i = 0; i < 7; i++)
            {
                commandToSend[i] = System.Convert.ToByte(textToShow[i]);
            }
            commandToSend[7] = 0x0D;
        }
        else
        {
            commandToSend[0] = 0;
            if (rbStatusOutOfRange.IsChecked == true)
                commandToSend[0] |= (byte)(1 << 4);
            if (rbStatusNegative.IsChecked == true)
                commandToSend[0] |= (byte)(1 << 3);
            if (rbStatusBalance.IsChecked == true)
                commandToSend[0] |= (byte)(1 << 2);
            if (rbStatusZero.IsChecked == true)
                commandToSend[0] |= (byte)(1 << 1);
            if (rbStatusNet.IsChecked == true)
                commandToSend[0] |= (byte)(1 << 0);

            var binary = Convert.ToString(commandToSend[0], 2);
            textToShow = string.Format("0b{0} {1,6:D6}", binary.PadLeft(8, '0'), (int)sPeso.Value);

            for (var i = 1; i < 7; i++)
            {
                commandToSend[i] = System.Convert.ToByte(textToShow[10 + i]);
            }
            commandToSend[7] = 0x0D;
        }

        tOutput.Text = textToShow + " 0x0D";
        SerialPort.Write(commandToSend, 0, 8);
    }
}
