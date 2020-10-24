#pragma once

namespace robosim
{
	namespace zmq_interface
	{
		void init();
		void step();
		float get_motor(int motor);
		void set_encoder(int encoder, float value);
		void set_accelerometer(int accelerometer, float value);
		void set_gyroscope(int gyroscope, float value);
		void set_advanced(int advanced, float value);
		void destroy();
	}
}
