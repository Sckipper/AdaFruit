#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "Adafruit_BLEGatt.h"
#include <Adafruit_Sensor.h>
#include <Adafruit_LSM303.h>

#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif

#define VERBOSE_MODE                   false 
#define BLUEFRUIT_UART_MODE_PIN        12    

Adafruit_LSM303 lsm303;
Adafruit_BluefruitLE_UART ble(Serial1, BLUEFRUIT_UART_MODE_PIN);
Adafruit_BLEGatt gatt(ble);

int32_t AccelCharId;
int32_t MagCharId;
byte result[12];

// Helper region
void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}

typedef union
{
 float nr;
 uint8_t by[4];
} FLOATUNION_t;
// END Helper region

FLOATUNION_t accelX;
FLOATUNION_t accelY;
FLOATUNION_t accelZ;
FLOATUNION_t magX;
FLOATUNION_t magY;
FLOATUNION_t magZ;

void setup(void)
{
//  while (!Serial); // required for Flora & Micro
//  delay(500);

  Serial.begin(115200);

  // Enable motion sensor
  Serial.println("Initialising the LSM303 module");
  if (!lsm303.begin())
      error(F("Unable to initialize LSM303. Check your wiring!"));
   Serial.println("Found LSM303");

  /* Initialise the module */
  Serial.print(F("Initialising the Bluefruit LE module: "));
  if (!ble.begin(VERBOSE_MODE))
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  Serial.println( F("Found Bluefruit LE") );

  /* Perform a factory reset to make sure everything is in a known state */
  Serial.println(F("Performing a factory reset: "));
  if (!ble.factoryReset())
       error(F("Couldn't factory reset"));

  /* Disable command echo from Bluefruit */
  ble.echo(false);

  Serial.println("Requesting Bluefruit info:");
  ble.info(); //Print Bluefruit information
  
  gatt.clear(); // clear all custom gatt
  // this line is particularly required for Flora, but is a good idea
  // anyways for the super long lines ahead!
  ble.setInterCharWriteDelay(5); // 5 ms

  //Accelerometer
  Serial.println(F(" Add Sensor service"));
  if (gatt.addService(0x3000) == 0)
    error(F("Fail"));
    
  Serial.println(F("Adding the accelerometer characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3001, PROPERTIES=0x02, MIN_LEN=12, MAX_LEN=12, VALUE=0"), &AccelCharId))
    error(F("Fail"));
    
  Serial.println(F("Adding the Magnetometer characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3002, PROPERTIES=0x02, MIN_LEN=12, MAX_LEN=12, VALUE=0"), &MagCharId))
    error(F("Fail"));
    
  /* Reset the device for the new service setting changes to take effect */
  Serial.println(F("Performing a SW reset (service changes require a reset): "));
  ble.reset();

  Serial.println( F("Finish Initialization, start sending data!") );
}


void loop(void)
{ 
  lsm303.read();

  accelX.nr = lsm303.accelData.x;
  accelY.nr = lsm303.accelData.y;
  accelZ.nr = lsm303.accelData.z;
  magX.nr = lsm303.magData.x;
  magY.nr = lsm303.magData.y;
  magZ.nr = lsm303.magData.z;
  
  memcpy(result, accelX.by, 4);
  memcpy(result+4, accelY.by, 4);
  memcpy(result+8, accelZ.by, 4);
  gatt.setChar(AccelCharId, result, 12);
  memcpy(result, magX.by, 4);
  memcpy(result+4, magY.by, 4);
  memcpy(result+8, magZ.by, 4);
  gatt.setChar(MagCharId, magX.by, 12);

//  Serial.print("Accel X: "); Serial.print(accelX.nr); Serial.print(" ");
//  Serial.print("Y: "); Serial.print(accelY.nr);       Serial.print(" ");
//  Serial.print("Z: "); Serial.println(accelZ.nr);     Serial.print(" ");
//  Serial.print("Mag X: "); Serial.print(magX.nr);     Serial.print(" ");
//  Serial.print("Y: "); Serial.print(magY.nr);         Serial.print(" ");
//  Serial.print("Z: "); Serial.println(magZ.nr);       Serial.print(" ");
}
