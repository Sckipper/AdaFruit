#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "Adafruit_LSM9DS0.h"
#include "BluefruitConfig.h"

#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif



//#include <Arduino.h>
//#include <Wire.h>
//#include <SPI.h>

//#include <Adafruit_Sensor.h>  // not used in this demo but required!
//#include "Adafruit_BLE.h"
//#include "Adafruit_BluefruitLE_SPI.h"
//#include "Adafruit_BluefruitLE_UART.h"
//#if SOFTWARE_SERIAL_AVAILABLE
//  #include <SoftwareSerial.h>
//#endif



Adafruit_LSM9DS0 lsm = Adafruit_LSM9DS0();
Adafruit_BluefruitLE_UART ble(Serial1, BLUEFRUIT_UART_MODE_PIN);


void setupSensor()
{
  // 1.) Set the accelerometer range
  lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_2G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_4G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_6G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_8G);
  //lsm.setupAccel(lsm.LSM9DS0_ACCELRANGE_16G);
  
  // 2.) Set the magnetometer sensitivity
  lsm.setupMag(lsm.LSM9DS0_MAGGAIN_2GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_4GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_8GAUSS);
  //lsm.setupMag(lsm.LSM9DS0_MAGGAIN_12GAUSS);

  // 3.) Setup the gyroscope
  lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_245DPS);
  //lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_500DPS);
  //lsm.setupGyro(lsm.LSM9DS0_GYROSCALE_2000DPS);
}

// A small helper
void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}

char * IntToChar(int number)
{
  int n = log10(number) + 1;
  int i;
  char *numberArray = calloc(n, sizeof(char));
  for ( i = 0; i < n; ++i, number /= 10 ){
      numberArray[i] = number % 10;
  }
  return numberArray;
}

void setup() 
{
  #ifndef ESP8266
    while (!Serial);     // will pause Zero, Leonardo, etc until serial console opens
  #endif


  
  Serial.println(isOK);
  isOK = ble.atcommand("AT+FACTORYRESET");
  Serial.println(isOK);
  Serial.println("Initialising the GATT module");
  isOK = ble.atcommand("AT+GATTCLEAR");
  Serial.println(isOK);
  isOK = ble.atcommand("AT+GATTADDSERVICE=UUID=0x180F");
  Serial.println(isOK);
  isOK = ble.atcommand("AT+GATTADDCHAR=UUID=0x2A19,PROPERTIES=0x10,MIN_LEN=1,VALUE=100");
  Serial.println(isOK);

  // Enable motion sensor
  Serial.println("Initialising the 9DOF module");
  if (!lsm.begin())
  {
    error(F("Oops ... unable to initialize the LSM9DS0. Check your wiring!"));
    while (1);
  }
  Serial.println("Found LSM9DS0 9DOF");
  
  // Enable Bluetooth sender
  Serial.print(F("Initialising the Bluefruit LE module: "));
  if ( !ble.begin(VERBOSE_MODE) )
  {
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  }
  Serial.println( F("OK!") );
  
  Serial.println("");
  Serial.println("");
}

void loop() 
{
  lsm.read();
  int accelX = (int)lsm.accelData.x;
  String message = "AT+BLEUARTTX=" + String(accelX);
  ble.println(message);
  //Serial.print("Accel X: "); Serial.print(accelX); Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.accelData.y);       Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.accelData.z);     Serial.print(" ");
//  Serial.print("Mag X: "); Serial.print((int)lsm.magData.x);     Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.magData.y);         Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.magData.z);       Serial.print(" ");
//  Serial.print("Gyro X: "); Serial.print((int)lsm.gyroData.x);   Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.gyroData.y);        Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.gyroData.z);      Serial.println(" ");
//  Serial.print("Temp: "); Serial.print((int)lsm.temperature);    Serial.println(" ");
  delay(200);
}
