#include "zmqInterface.h"
#define ZMQ_STATIC
#include "zmq.h"

#include <map>
#include <thread>
#include <mutex>

#include "ControlMessage.pb.h"
#include "StatusMessage.pb.h"

static void* robosim_zmq_context = NULL;
static void* publisher_socket = NULL;
static void* replyer_socket = NULL;

static std::map<int, float> motor_map;
static std::map<int, float> encoder_map;
static std::map<int, float> accelerometer_map;
static std::map<int, float> gyroscope_map;
static std::map<int, float> advanced_map;

static std::mutex guard_mutex;

static void reinitialize_publisher()
{
	zmq_close(publisher_socket);
	publisher_socket = NULL;
	publisher_socket = zmq_socket(robosim_zmq_context, ZMQ_PUB);
	int rc = zmq_bind(publisher_socket, "tcp://*:10502");
	if (rc <= -1)
	{
		publisher_socket = NULL;
	}
}

static void reinitialize_replyer()
{
	zmq_close(replyer_socket);
	replyer_socket = NULL;
	replyer_socket = zmq_socket(robosim_zmq_context, ZMQ_REP);
	int rc = zmq_bind(replyer_socket, "tcp://*:10501");
	if (rc <= -1)
	{
		replyer_socket = NULL;
	}
}

void robosim::zmq_interface::init()
{
	robosim_zmq_context = zmq_ctx_new();
	reinitialize_publisher();
	reinitialize_replyer();
}

void robosim::zmq_interface::step()
{
	static int publisher_fail_counter = 0;
	static int replyer_fail_counter = 0;

	if (publisher_socket != NULL)
	{
		StatusMessage robosimStatus;
		
		{
			const std::lock_guard<std::mutex> lock(guard_mutex);

			for (std::map<int, float>::iterator i = encoder_map.begin();
				i != encoder_map.end();
				i++)
			{
				ValueMessage* temp =robosimStatus.add_encoders();
				temp->set_id((*i).first);
				temp->set_value((*i).second);
			}
			for (std::map<int, float>::iterator i = accelerometer_map.begin();
				i != accelerometer_map.end();
				i++)
			{
				ValueMessage* temp = robosimStatus.add_accelerometers();
				temp->set_id((*i).first);
				temp->set_value((*i).second);
			}

			for (std::map<int, float>::iterator i = gyroscope_map.begin();
				i != gyroscope_map.end();
				i++)
			{
				ValueMessage* temp = robosimStatus.add_gyroscopes();
				temp->set_id((*i).first);
				temp->set_value((*i).second);
			}

			for (std::map<int, float>::iterator i = advanced_map.begin();
				i != advanced_map.end();
				i++)
			{
				ValueMessage* temp = robosimStatus.add_advanced();
				temp->set_id((*i).first);
				temp->set_value((*i).second);
			}
		}

		static char publisher_buffer[16000];

		robosimStatus.SerializeToArray(publisher_buffer, 16000);

		zmq_send(publisher_socket, publisher_buffer, robosimStatus.ByteSizeLong(), 0);
	}
	else
	{
		publisher_fail_counter = (publisher_fail_counter++) % 100;
		if (publisher_fail_counter == 99)
			reinitialize_publisher();
	}

	if (replyer_socket != NULL)
	{
		static char replyer_buffer[16000];

		ControlMessage robosimControl;
		int received_bytes = zmq_recv(replyer_socket, replyer_buffer, 16000, ZMQ_NOBLOCK);
		if (received_bytes > 0)
		{
			{
				const std::lock_guard<std::mutex> lock(guard_mutex);
				robosimControl.ParseFromArray(replyer_buffer, 16000);

				for (int i = 0; i < robosimControl.motors_size(); i++)
				{
					motor_map[robosimControl.motors(i).id()] = robosimControl.motors(i).value();
				}
			}
			zmq_send(replyer_socket, "A", 1, 0);
		}
	}
	else
	{
		replyer_fail_counter = (replyer_fail_counter++) % 100;
		if (replyer_fail_counter == 99)
			reinitialize_replyer();
	}
}

float robosim::zmq_interface::get_motor(int motor)
{
	const std::lock_guard<std::mutex> lock(guard_mutex);
	return motor_map[motor];
}

void robosim::zmq_interface::set_encoder(int encoder, float value)
{
	const std::lock_guard<std::mutex> lock(guard_mutex);
	encoder_map[encoder] = value;
}

void robosim::zmq_interface::set_accelerometer(int accelerometer, float value)
{
	const std::lock_guard<std::mutex> lock(guard_mutex);
	accelerometer_map[accelerometer] = value;
}

void robosim::zmq_interface::set_gyroscope(int gyroscope, float value)
{
	const std::lock_guard<std::mutex> lock(guard_mutex);
	gyroscope_map[gyroscope] = value;
}

void robosim::zmq_interface::set_advanced(int advanced, float value)
{
	const std::lock_guard<std::mutex> lock(guard_mutex);
	advanced_map[advanced] = value;
}

void robosim::zmq_interface::destroy()
{
	zmq_close(publisher_socket);
	zmq_close(replyer_socket);
	zmq_ctx_destroy(robosim_zmq_context);
	publisher_socket = NULL;
	replyer_socket = NULL;
	robosim_zmq_context = NULL;
}
