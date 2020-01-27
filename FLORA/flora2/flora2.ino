#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "Adafruit_BLEGatt.h"
#include "Adafruit_LSM9DS0.h"

#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif


#define VERBOSE_MODE                   false 
#define BLUEFRUIT_UART_MODE_PIN        12    

Adafruit_LSM9DS0 lsm = Adafruit_LSM9DS0();
Adafruit_BluefruitLE_UART ble(Serial1, BLUEFRUIT_UART_MODE_PIN);
Adafruit_BLEGatt gatt(ble);

int32_t AccelServiceId;
int32_t AccelXCharId;
int32_t AccelYCharId;
int32_t AccelZCharId;

int32_t MagServiceId;
int32_t MagXCharId;
int32_t MagYCharId;
int32_t MagZCharId;

int32_t GyroServiceId;
int32_t GyroXCharId;
int32_t GyroYCharId;
int32_t GyroZCharId;

// Helper region
void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}

typedef union
{
 float nr;
 uint8_t by[5];
} FLOATUNION_t;
// END Helper region

FLOATUNION_t accelX;
FLOATUNION_t accelY;
FLOATUNION_t accelZ;
FLOATUNION_t magX;
FLOATUNION_t magY;
FLOATUNION_t magZ;
FLOATUNION_t gyroX;
FLOATUNION_t gyroY;
FLOATUNION_t gyroZ;

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

void setup(void)
{
//  while (!Serial); // required for Flora & Micro
//  delay(500);

  Serial.begin(115200);

  // Enable motion sensor
  Serial.println("Initialising the 9DOF module");
  if (!lsm.begin())
    error(F("Oops ... unable to initialize the LSM9DS0. Check your wiring!"));
  Serial.println("Found LSM9DS0 9DOF");
  
  /* Initialise the module */
  Serial.print(F("Initialising the Bluefruit LE module: "));
  if ( !ble.begin(VERBOSE_MODE) )
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  Serial.println( F("OK!") );

  /* Perform a factory reset to make sure everything is in a known state */
  Serial.println(F("Performing a factory reset: "));
  if (! ble.factoryReset() )
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
  Serial.println(F(" Add Accelerometer service"));
  AccelServiceId = gatt.addService(0x3000);
  if (AccelServiceId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the accelerometer X characteristic"));
  AccelXCharId = gatt.addCharacteristic(0x3001, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (AccelXCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the accelerometer Y characteristic"));
  AccelYCharId = gatt.addCharacteristic(0x3002, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (AccelYCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the accelerometer Z characteristic"));
  AccelZCharId = gatt.addCharacteristic(0x3003, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (AccelZCharId == 0)
    error(F("Fail"));

  // Gyroscope
  Serial.println(F(" Add Gyroscope service"));
  GyroServiceId = gatt.addService(0x3004);
  if (GyroServiceId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope X characteristic"));
  GyroXCharId = gatt.addCharacteristic(0x3005, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (GyroXCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope Y characteristic"));
  GyroYCharId = gatt.addCharacteristic(0x3006, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (GyroYCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope Z characteristic"));
  GyroZCharId = gatt.addCharacteristic(0x3007, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (GyroZCharId == 0)
    error(F("Fail"));

  //Magnetometer
  Serial.println(F(" Add Magnetometer service"));
  MagServiceId = gatt.addService(0x3008);
  if (MagServiceId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer X characteristic"));
  MagXCharId = gatt.addCharacteristic(0x3009, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (MagXCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer Y characteristic"));
  MagYCharId = gatt.addCharacteristic(0x3010, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (MagYCharId == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer Z characteristic"));
  MagZCharId = gatt.addCharacteristic(0x3011, GATT_CHARS_PROPERTIES_INDICATE, 5, 5, BLE_DATATYPE_BYTEARRAY);
  if (MagZCharId == 0)
    error(F("Fail"));
    
  /* Reset the device for the new service setting changes to take effect */
  Serial.println(F("Performing a SW reset (service changes require a reset): "));
  ble.reset();

  Serial.println( F("Finish Initialization, start sending data!") );
}


void loop(void)
{ 
  lsm.read();
  
//  Serial.print("Accel X: "); Serial.print((int)lsm.accelData.x); Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.accelData.y);       Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.accelData.z);     Serial.print(" ");
//  Serial.print("Mag X: "); Serial.print((int)lsm.magData.x);     Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.magData.y);         Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.magData.z);       Serial.print(" ");
//  Serial.print("Gyro X: "); Serial.print((int)lsm.gyroData.x);   Serial.print(" ");
//  Serial.print("Y: "); Serial.print((int)lsm.gyroData.y);        Serial.print(" ");
//  Serial.print("Z: "); Serial.println((int)lsm.gyroData.z);      Serial.println(" ");
//  Serial.print("Temp: "); Serial.print((int)lsm.temperature);    Serial.println(" ");
  
  accelX.nr = lsm.accelData.x;
  accelY.nr = lsm.accelData.y;
  accelZ.nr = lsm.accelData.z;
  magX.nr = lsm.magData.x;
  magY.nr = lsm.magData.y;
  magZ.nr = lsm.magData.z;
  gyroX.nr = lsm.gyroData.x;
  gyroY.nr = lsm.gyroData.y;
  gyroZ.nr = lsm.gyroData.z;

  gatt.setChar(AccelXCharId, accelX.by, 5);
  gatt.setChar(AccelYCharId, accelY.by, 5);
  gatt.setChar(AccelZCharId, accelZ.by, 5);
  
  gatt.setChar(MagXCharId, magX.by, 5);
  gatt.setChar(MagYCharId, magY.by, 5);
  gatt.setChar(MagZCharId, magZ.by, 5);
  
  gatt.setChar(GyroXCharId, gyroX.by, 5);
  gatt.setChar(GyroYCharId, gyroY.by, 5);
  gatt.setChar(GyroZCharId, gyroZ.by, 5);
}
