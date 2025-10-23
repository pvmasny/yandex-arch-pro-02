using events.Dto;
using events.Infrastructure.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace events.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;

        private readonly KafkaProducer _kafkaProducer;

        public EventsController(ILogger<EventsController> logger,
            KafkaProducer kafkaProducer)
        {
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }

        [HttpPost("movie")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieDto movieDto)
        {
            await _kafkaProducer.ProduceAsync("movie-events", JsonSerializer.Serialize(movieDto));
            return Ok();
        }

        [HttpPost("user")]
        public async Task<IActionResult> UserMovie([FromBody] UserDto userDto)
        {
            await _kafkaProducer.ProduceAsync("user-events", JsonSerializer.Serialize(userDto));
            return Ok();
        }

        [HttpPost("payment")]
        public async Task<IActionResult> PaymentMovie([FromBody] PaymentDto paymentDto)
        {
            await _kafkaProducer.ProduceAsync("payment-events", JsonSerializer.Serialize(paymentDto));
            return Ok();
        }


    }
}
