#include <Sodaq_RN2483.h>
#include <Wire.h>
#include <Sodaq_BMP085.h>
#include <Sodaq_SHT2x.h>
////SETTINGS///
// MBili / Tatu
//#define debugSerial Serial
//#define beePin 20
// Autonomo
#define debugSerial SerialUSB
#define beePin BEE_VCC
////END SETTINGS////
// LoRaBEE
#define loraSerial Serial1
const uint8_t devAddr[4] =
{
  //0x00, 0x00, 0x00, 0x00
  //02017401
  0x02, 0x01, 0x74, 0x01
};
// USE YOUR OWN KEYS!
const uint8_t appSKey[16] =
{
  0x2B, 0x7E, 0x15, 0x16, 0x28, 0xAE, 0xD2, 0xA6, 0xAB, 0xF7, 0x15, 0x88, 0x09, 0xCF, 0x4F, 0x3C
  //0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};
// USE YOUR OWN KEYS!
const uint8_t nwkSKey[16] =
{
  0x2B, 0x7E, 0x15, 0x16, 0x28, 0xAE, 0xD2, 0xA6, 0xAB, 0xF7, 0x15, 0x88, 0x09, 0xCF, 0x4F, 0x3C
  //0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};
//TPH BMP sensor
Sodaq_BMP085 bmp;
void setup()
{
  // REMOVE WHEN YOU DONT USE PC
  while(!debugSerial);
  
  debugSerial.begin(57600);
  loraSerial.begin(LoRaBee.getDefaultBaudRate());
  digitalWrite(beePin, HIGH);
  LoRaBee.setDiag(debugSerial); // optional
  if (LoRaBee.initABP(loraSerial, devAddr, appSKey, nwkSKey, false))
  {
    debugSerial.println("Connection to the network was successful.");
  }
  else
  {
    debugSerial.println("Connection to the network failed!");
  }
  setupTPH();
}

void loop()
{
  debugSerial.println("Sending payload: TempSHT21, TempBMP, PressureBMP, HumiditySHT21");
  String reading = takeTPHReading();
  //sendPayload((uint8_t*)reading.c_str(), reading.length());
  debugSerial.println();
    switch (LoRaBee.sendReqAck(1, (uint8_t*)reading.c_str(), reading.length(), 8))
    {
    case NoError:
      debugSerial.println("Successful transmission.");
      break;
    case NoResponse:
      debugSerial.println("There was no response from the device.");
      break;
    case Timeout:
      debugSerial.println("Connection timed-out. Check your serial connection to the device! Sleeping for 20sec.");
      delay(20000);
      break;
    case PayloadSizeError:
      debugSerial.println("The size of the payload is greater than allowed. Transmission failed!");
      break;
    case InternalError:
      debugSerial.println("Oh No! This shouldn't happen. Something is really wrong! Try restarting the device!rnThe program will now halt.");
      while (1) {};
      break;
    case Busy:
      debugSerial.println("The device is busy. Sleeping for 10 extra seconds.");
      delay(10000);
      break;
    case NetworkFatalError:
      debugSerial.println("There is a non-recoverable error with the network connection. You should re-connect.rnThe program will now halt.");
      while (1) {};
      break;
    case NotConnected:
      debugSerial.println("The device is not connected to the network. Please connect to the network before attempting to send data.rnThe program will now halt.");
      while (1) {};
      break;
    case NoAcknowledgment:
      debugSerial.println("There was no acknowledgment sent back!");
      break;
    default:
      break;
    }
    // Delay between readings
    // 60 000 = 1 minute
    delay(10000);
  
}
void setupTPH()
{
  //Initialise the wire protocol for the TPH sensors
  Wire.begin();
  
  //Initialise the TPH BMP sensor
  bmp.begin();
}
String takeTPHReading()
{
  //Create a String type data record in csv format
  //TempSHT21, TempBMP, PressureBMP, HumiditySHT21
  /*
  String data = String(SHT2x.GetTemperature())  + ", ";
  data += String(bmp.readTemperature()) + ", ";
  data += String(bmp.readPressure() / 100)  + ", ";
  data += String(SHT2x.GetHumidity());
  */
  String data = "{ \"I\": 0, \"S\": 7.5, \"R\": 6 }";
  debugSerial.println(data);
  
  return data;
}
