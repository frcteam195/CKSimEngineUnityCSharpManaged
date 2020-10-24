#pragma once

namespace robosim
{
	float Get_Motor(int motor_id);

	void Set_Encoder(int encoder_id, float value);

	void Set_Accelerometer(int accelerometer_id, float value);

	void Set_Gyroscope(int gyroscope_id, float value);

	void Set_Advanced(int advanced_id, float value);
}
