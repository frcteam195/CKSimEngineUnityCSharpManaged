#include "RoboSim.h"
#include "zmqInterface.h"

extern "C"
{
    __declspec(dllexport) void robosim_zmq_interface_destroy() { robosim::zmq_interface::destroy(); }

    __declspec(dllexport) void robosim_zmq_interface_init() { robosim::zmq_interface::init(); }

    __declspec(dllexport) void robosim_zmq_interface_step() { robosim::zmq_interface::step(); }

    __declspec(dllexport) void robosim_get_motor(int motor_id) { robosim::Get_Motor(motor_id); }

    __declspec(dllexport) void robosim_set_encoder(int encoder_id, float value) { robosim::Set_Encoder(encoder_id, value); }

    __declspec(dllexport) void robosim_set_accelerometer(int accelerometer_id, float value) { robosim::Set_Accelerometer(accelerometer_id, value); }

    __declspec(dllexport) void robosim_set_gyroscope(int gyroscope_id, float value) { robosim::Set_Gyroscope(gyroscope_id, value); }

    __declspec(dllexport) void robosim_set_advanced(int advanced_id, float value) { robosim::Set_Advanced(advanced_id, value); }
}