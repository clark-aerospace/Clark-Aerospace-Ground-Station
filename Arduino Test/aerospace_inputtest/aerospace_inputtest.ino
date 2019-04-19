int alt = 0;
int temp = 0;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  alt += 1;
  // put your main code here, to run repeatedly:
  char altBuffer[20];
  char tempBuffer[20];

  TestTempSine();
  
  sprintf(altBuffer, "ALTITUDE:%i", alt);
  sprintf(tempBuffer, "PAYLOAD_TEMP:%i", temp);

  Serial.println(altBuffer);
  Serial.println(tempBuffer);
  //Serial.flush();
  delay(100);
}

void TestTempSine() {
  if (temp < 50) {
    temp += 5;
  } else {
    temp = 10;
  }
}
