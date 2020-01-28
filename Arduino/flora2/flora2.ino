#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "Adafruit_BLEGatt.h"
#include "Adafruit_LSM9DS0.h"

#include <Adafruit_Sensor.h>
#include <Adafruit_LSM303.h>

#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif

#define VERBOSE_MODE                   false 
#define BLUEFRUIT_UART_MODE_PIN        12    

bool isLSM303 = false;
Adafruit_LSM9DS0 lsm = Adafruit_LSM9DS0();
Adafruit_LSM303 lsm303;
Adafruit_BluefruitLE_UART ble(Serial1, BLUEFRUIT_UART_MODE_PIN);
Adafruit_BLEGatt gatt(ble);

int32_t AccelXCharId;
int32_t AccelYCharId;
int32_t AccelZCharId;

int32_t MagXCharId;
int32_t MagYCharId;
int32_t MagZCharId;

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
  if (!lsm.begin()){
    Serial.println(F("Unable to initialize the LSM9DS0. Trying LSM303"));
    isLSM303 = true;
    if (!lsm303.begin())
      error(F("Unable to initialize LSM9DS0 and LSM303. Check your wiring!"));
  }else {
    Serial.println("Found LSM9DS0 9DOF");
  }
    
  
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
  if (gatt.addService(0x3000) == 0)
    error(F("Fail"));

  bool success = false;
  Serial.println(F("Adding the accelerometer X characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3001, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &AccelXCharId))
    error(F("Fail"));

  Serial.println(F("Adding the accelerometer Y characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3002, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &AccelYCharId))
    error(F("Fail"));

  Serial.println(F("Adding the accelerometer Z characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3003, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &AccelZCharId))
    error(F("Fail"));

  // Gyroscope
  Serial.println(F(" Add Gyroscope service"));
  if (gatt.addService(0x3004) == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope X characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3005, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &GyroXCharId))
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope Y characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3006, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &GyroYCharId))
    error(F("Fail"));

  Serial.println(F("Adding the Gyroscope Z characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3007, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &GyroZCharId))
    error(F("Fail"));

  //Magnetometer
  Serial.println(F(" Add Magnetometer service"));
  if (gatt.addService(0x3008) == 0)
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer X characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3009, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &MagXCharId))
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer Y characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3010, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &MagYCharId))
    error(F("Fail"));

  Serial.println(F("Adding the Magnetometer Z characteristic"));
  if (! ble.sendCommandWithIntReply( F("AT+GATTADDCHAR=UUID=0x3011, PROPERTIES=0x02, MIN_LEN=5, MAX_LEN=5, VALUE=0"), &MagZCharId))
    error(F("Fail"));
    
  /* Reset the device for the new service setting changes to take effect */
  Serial.println(F("Performing a SW reset (service changes require a reset): "));
  ble.reset();

  Serial.println( F("Finish Initialization, start sending data!") );
}


void loop(void)
{ 
  if(!isLSM303){
      lsm.read();
      accelX.nr = lsm.accelData.x;
      accelY.nr = lsm.accelData.y;
      accelZ.nr = lsm.accelData.z;
      magX.nr = lsm.magData.x;
      magY.nr = lsm.magData.y;
      magZ.nr = lsm.magData.z;
      gyroX.nr = lsm.gyroData.x;
      gyroY.nr = lsm.gyroData.y;
      gyroZ.nr = lsm.gyroData.z;
  }else {
      lsm303.read();
      accelX.nr = lsm303.accelData.x;
      accelY.nr = lsm303.accelData.y;
      accelZ.nr = lsm303.accelData.z;
      magX.nr = lsm303.magData.x;
      magY.nr = lsm303.magData.y;
      magZ.nr = lsm303.magData.z; 
      gyroX.nr = 0;
      gyroY.nr = 0;
      gyroZ.nr = 0;
  }
  
  gatt.setChar(AccelXCharId, accelX.by, 5);
  gatt.setChar(AccelYCharId, accelY.by, 5);
  gatt.setChar(AccelZCharId, accelZ.by, 5);
  
  gatt.setChar(MagXCharId, magX.by, 5);
  gatt.setChar(MagYCharId, magY.by, 5);
  gatt.setChar(MagZCharId, magZ.by, 5);
  
  gatt.setChar(GyroXCharId, gyroX.by, 5);
  gatt.setChar(GyroYCharId, gyroY.by, 5);
  gatt.setChar(GyroZCharId, gyroZ.by, 5);

//  Serial.print("Accel X: "); Serial.print(accelX.nr); Serial.print(" ");
//  Serial.print("Y: "); Serial.print(accelY.nr);       Serial.print(" ");
//  Serial.print("Z: "); Serial.println(accelZ.nr);     Serial.print(" ");
//  Serial.print("Mag X: "); Serial.print(magX.nr);     Serial.print(" ");
//  Serial.print("Y: "); Serial.print(magY.nr);         Serial.print(" ");
//  Serial.print("Z: "); Serial.println(magZ.nr);       Serial.print(" ");
//  Serial.print("Gyro X: "); Serial.print(gyroX.nr);   Serial.print(" ");
//  Serial.print("Y: "); Serial.print(gyroY.nr);        Serial.print(" ");
//  Serial.print("Z: "); Serial.println(gyroZ.nr);      Serial.println(" ");
}
