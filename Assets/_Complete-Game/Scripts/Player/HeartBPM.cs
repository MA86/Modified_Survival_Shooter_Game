//// *************************************************************************************************************************** ////
                                                    
                                                 //// HEART RATE ALGORITHM PROTOTYPE ////

// This algorithm translates the raw signal coming from the PulseSensor device into heart rate as beats per minute (BPM).

// This algorithm was designed for the Survival Shooter game, but it can also be used for other game projects. To use this algorithm, 
// simply add it as a component to the Player object of the Survival Shooter. You will need an open-source PulseSensor to read your 
// heart beat. Since the PulseSensor cannot directly connect to USB port, you will also need an Arduino Uno.

// Arduino Uno simply converts the electrical signal, coming from the PulseSensor, to digital signal. Connect PulseSensor
// to Arduino Uno, and connect Arduino Uno to your PC's USB port. PulseSensor can be purchased from: https://pulsesensor.com/ 

// To easily understand this algorithm, please read the comments next to each line of code below. It'll help you modify this algorithm 
// further, if needed.

//// **************************************************************************************************************************** ////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;          // To use Unity
using System;
using System.IO.Ports;      // To use USB ports
using System.IO;


public class HeartBPM : MonoBehaviour
{
    int signal;                         // This variable will hold the raw signal
    const int THRESHOLD = 540;          // This variable holds 'threshold signal', anything above is considered a heart beat
    string state = "in rest";           // Used for setting 'in rest' or 'in beat' states; initially we are 'in rest' state
    DateTime timept1 = DateTime.Now;    // Times the first beat
    DateTime timept2 = DateTime.Now;    // Times the second beat
    DateTime timept3 = DateTime.Now;    // Times the third beat
    DateTime timept4 = DateTime.Now;    // So on and so forth... main idea is to measure 'instantaneous' heart beats
    DateTime timept5 = DateTime.Now;    // ...
    DateTime timept6 = DateTime.Now;    // ...
    DateTime timept7 = DateTime.Now;    // ...
    DateTime timept8 = DateTime.Now;    // ...
    
    public Double beatsPerMinute;                            // To hold heart rate as BPM

    SerialPort serialPort = new SerialPort("COM3", 9600);                       // Create a USB port object
    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();    // Create a stopwatch object


    public int baselineBPM;     // To hold player's 'baseline' or normal heart rate

    // Start function is called before the first frame of videogame update
    void Start()
    {
        serialPort.Open();    // Open the USB port for reading the PulseSensor data
        watch.Start();        // Start the stopwatch to measure player's BASELINE heart beat


        // Baseline heart rate is stored in savedBaseline.txt. This is to prevent RECALCULATING the SAME player's basline heart rate
        // over and over each time player dies. Logic is as follows; if there is a number in the file, assign it to baselineBPM. 
        if (!(new FileInfo("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt").Length == 0))
        {
            StreamReader fileReader = new StreamReader("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt");
            baselineBPM = Convert.ToInt16(fileReader.ReadLine());
            fileReader.Close();

        }
    }

    // Update function is called once per frame of videogame 
    void FixedUpdate()
    {
        if (!serialPort.IsOpen)     // If USB port is close, just open it
            serialPort.Open();

        if (serialPort.BytesToRead == 0)    // If the USB buffer has no signal data in it, check again in the next frame
            return;

        if (state == "in rest")     // Initially, the sensor is considered not in use, so the state is 'in rest'!
        {
            int.TryParse(serialPort.ReadLine(), out signal);     // Read a signal from the USB port buffer!
            serialPort.DiscardInBuffer();                        // Clear the USB buffer to discard old data

            if (signal > THRESHOLD)     // If signal is greater than 540, it must be a heart beat!
            {
                TimeSpan deltaTime = DateTime.Now - timept1;    // Get the total time elapsed since the FIRST heart beat

                // Shift the time points for the NEXT 8 beats to be calcualted, ONE to the right! //
                // The idea here is to calculate instantaneous BPM using 8 beats, then move the frame by one beat //
                timept1 = timept2;           // Time for beat 1 is now time for beat 2 
                timept2 = timept3;           // Time for beat 2 is now time for beat 3
                timept3 = timept4;           // Time for beat 3 is now time for beat 4... and so on and so forth
                timept4 = timept5;
                timept5 = timept6;
                timept6 = timept7;
                timept7 = timept8;
                timept8 = DateTime.Now;      // frame shift complete!

                beatsPerMinute = 8 / deltaTime.TotalMinutes;     // Calculate the BPM for the last 8 beats
                Debug.Log((int)beatsPerMinute);                  // Output the heart rate in Unity Editor console for debugging
                state = "in beat";                               // Reset the state to indicate that player's heart is in BEAT MODE!

                // Below, player's 'baseline' heart rate is calculated because each player's normal heart rate can vary //
                bool underAMinute = (watch.ElapsedMilliseconds / 1000.0 / 60.0) < 1.0;      // Has it been a minute?

                // If it has been a minute and file is empty, count the heartbeat in baselineBPM
                if (underAMinute && (new FileInfo("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt").Length == 0))
                {
                    baselineBPM++;
                }
                else
                {
                    if(new FileInfo("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt").Length == 0)
                    {
                        string baselineBPMToFile = baselineBPM.ToString();
                        File.WriteAllText("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt", baselineBPMToFile);
                    }
                    
                    watch.Stop();
                }

            }

        }
        else    // If state is 'in beat' mode, then simply do the following:
        {
            int.TryParse(serialPort.ReadLine(), out signal);     // Read a signal

            if (signal < THRESHOLD)     // If signal is less than 540, device is 'in rest' mode (AKA not being used!)
            {
                state = "in rest";      // If so, then reset state to 'in rest' to indicate that the heart beat is NOT beating!
            }
        }
    }
}
