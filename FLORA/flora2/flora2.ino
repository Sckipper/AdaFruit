#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "Adafruit_BLEGatt.h"
#include "IEEE11073float.h"

#include "BluefruitConfig.h"

#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif
#define VBATPIN A9

Adafruit_BluefruitLE_UART ble(Serial1, BLUEFRUIT_UART_MODE_PIN);
Adafruit_BLEGatt gatt(ble);

// A small helper
void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}
typedef union
{
 float number;
 uint8_t bytes[5];
} FLOATUNION_t;

/* The service information */

int32_t batteryServiceId;
int32_t batteryCharId;


void setup(void)
{
  while (!Serial); // required for Flora & Micro
  delay(500);

  boolean success;

  Serial.begin(115200);
  Serial.println(F("Adafruit Bluefruit Health Thermometer Example"));
  Serial.println(F("--------------------------------------------"));

  randomSeed(micros());

  /* Initialise the module */
  Serial.print(F("Initialising the Bluefruit LE module: "));

  if ( !ble.begin(VERBOSE_MODE) )
  {
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  }
  Serial.println( F("OK!") );

  /* Perform a factory reset to make sure everything is in a known state */
  Serial.println(F("Performing a factory reset: "));
  if (! ble.factoryReset() ){
       error(F("Couldn't factory reset"));
  }

  /* Disable command echo from Bluefruit */
  ble.echo(false);

  Serial.println("Requesting Bluefruit info:");
  /* Print Bluefruit information */
  ble.info();


  gatt.clear();
  // this line is particularly required for Flora, but is a good idea
  // anyways for the super long lines ahead!
  ble.setInterCharWriteDelay(5); // 5 ms

  /* Add the Heart Rate Service definition */
  /* Service ID should be 1 */
  Serial.println(F(" Add a battery service (UUID = 0x180F) to the peripheral "));
  batteryServiceId = gatt.addService(0x180F);
  if (batteryServiceId == 0) {
    error(F("Could not add battery service"));
  }
  
  /* Add the Temperature Measurement characteristic which is composed of
   * 1 byte flags + 4 float */
  /* Chars ID for Measurement should be 1 */
  Serial.println(F("Adding the Battery Measurement characteristic (UUID = 0x2A1C): "));
  batteryCharId = gatt.addCharacteristic(0x2A19, GATT_CHARS_PROPERTIES_WRITE_WO_RESP, 5, 5, BLE_DATATYPE_INTEGER);
  if (batteryCharId == 0) {
    error(F("Could not add Battery characteristic"));
  }

//  /* Add the Health Thermometer Service to the advertising data (needed for Nordic apps to detect the service) */
//  Serial.print(F("Adding Health Thermometer Service UUID to the advertising payload: "));
//  uint8_t advdata[] { 0x02, 0x01, 0x06, 0x05, 0x02, 0x09, 0x18, 0x0a, 0x18 };
//  ble.setAdvData( advdata, sizeof(advdata) );

  /* Reset the device for the new service setting changes to take effect */
  Serial.print(F("Performing a SW reset (service changes require a reset): "));
  ble.reset();

  Serial.println();
}

/** Send randomized heart rate data continuously **/
void loop(void)
{
  FLOATUNION_t measuredvbat;
  measuredvbat.number = analogRead(VBATPIN);

  measuredvbat.number *= 2;    // we divided by 2, so multiply back
  measuredvbat.number *= 3.3;  // Multiply by 3.3V, our reference voltage
  measuredvbat.number /= 1024; // convert to voltage
  Serial.print("VBat: " ); Serial.println(measuredvbat.number);
  
  
  gatt.setChar(batteryCharId, measuredvbat.bytes, 5);
}
