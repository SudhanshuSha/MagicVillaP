//using AutoMapper;
//using MagicVilla_WebAPI.Data;
//using MagicVilla_WebAPI.Logging;
//using MagicVilla_WebAPI.Models;
//using MagicVilla_WebAPI.Models.Dto;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace MagicVilla_WebAPI.Controllers
//{

//    //Controller base contain the common method for returning all the data 
//    [Route("api/[controller]")]
//    // Notifies application it will be API controller
//    // and load some basic features into API controller
//    [ApiController]
//    public class VillaControllerSample : ControllerBase
//    {
//        //private readonly ILogger<VillaController> _logger;

//        // as logger is already registered because of depencdency injection .net core will provide the implementation
//        // 
//        //public VillaController(ILogger<VillaController> logger)
//        //{
//        //    _logger = logger;
//        //}


//        private readonly ILogging _logger;
//        private readonly ApllicationDbContext _context;
//        private readonly IMapper _mapper;
//        // Ilogging is not registered in the container
//        // we need to explicitly define as its not a built i container service
//        public VillaControllerSample(ILogging logger, ApllicationDbContext context, IMapper mapper)
//        {
//            _logger = logger;
//            _context = context;
//            _mapper = mapper;
//        }

//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaDTO>))]
//        public async Task<ActionResult<IEnumerable<VillaDTO>>> Villas()
//        {
//            //_logger.LogInformation("Getting all Villas");
//            _logger.Log("Getting all Villas", "");

//            IEnumerable<Villa> villas = await _context.Villas.ToListAsync();
//            return Ok(_mapper.Map<List<VillaDTO>>(villas));
//        }

//        [HttpGet("{id:int}", Name = "GetVilla")]
//        // we can documnent what are the response type this API can return
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        //[ProducesResponseType(200, Type = typeof(VillaDTO)]
//        //[ProducesResponseType(400)]
//        //[ProducesResponseType(404)]
//        public async Task<ActionResult<VillaDTO>> Villa(int id)
//        {
//            if (id == 0)
//            {
//                //_logger.LogError("Get Villa Error with id: " + id);

//                _logger.Log("Get Villa Error with id: " + id, "Error");
//                return BadRequest();
//            }
//            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
//            if (villa == null)
//            {
//                return NotFound();
//            }
//            return Ok(_mapper.Map<VillaDTO>(villa));
//        }

//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<VillaDTO>> Villa([FromBody] VillaCreateDTO createDTO)
//        {
//            if (await _context.Villas.FirstOrDefaultAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
//            {
//                ModelState.AddModelError("CustomError", "Villa already Exist");
//                return BadRequest();
//            }
//            if (ModelState.IsValid)
//            {
//                //Villa villa = new Villa()
//                //{
//                //    Amenity = createDTO.Amenity,
//                //    ImageUrl = createDTO.ImageUrl,
//                //    Details = createDTO.Details,
//                //    Name = =createDTO.Name,
//                //    Occupancy = createDTO.Occupancy,
//                //    Rate = createDTO.Rate,
//                //    Sqft = createDTO.Sqft,
//                //};

//                Villa villa = _mapper.Map<Villa>(createDTO);

//                // EF is keepring the track of everyhting it has to do
//                await _context.Villas.AddAsync(villa);
//                // to save Chnages Ef will gather all the chnages and push them
//                await _context.SaveChangesAsync();

//                // whe  we creating the villa EF core will autmaticllay popul;ate the id so we can use it diectley
//                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _mapper.Map<VillaDTO>(villa));
//            }
//            else
//            {
//                return BadRequest(ModelState);
//            }

//        }

//        [HttpDelete("{id:int}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<ActionResult> DeleteVilla(int id)
//        {
//            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
//            if (villa == null)
//            {
//                return NotFound();
//            }
//            _context.Villas.Remove(villa);
//            await _context.SaveChangesAsync();
//            return NoContent();

//        }

//        [HttpPut("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
//        {
//            if (updateDTO == null || updateDTO.Id != id)
//            {
//                return BadRequest();
//            }

//            //Villa villa = new Villa()
//            //{
//            //    Id = id,
//            //    Amenity = updateDTO.Amenity,
//            //    ImageUrl = updateDTO.ImageUrl,
//            //    Details = updateDTO.Details,
//            //    Name = updateDTO.Name,
//            //    Occupancy = updateDTO.Occupancy,
//            //    Rate = updateDTO.Rate,
//            //    Sqft = updateDTO.Sqft,
//            //};

//            Villa villa = _mapper.Map<Villa>(updateDTO);

//            _context.Villas.Update(villa);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        // when we working with patch we will receive patch document of type VillaDto
//        public async Task<ActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
//        {
//            if (id == 0 || patchDTO == null)
//            {
//                return BadRequest();
//            }

//            // when we run first or default it actually keep track of that entity

//            // here we dont want entity frame work to track the object 
//            var villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

//            if (villa == null)
//            {
//                return NotFound();
//            }

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            // it update the name as object is being tracked 
//            //villa.Name = "Demo";
//            //_context.SaveChanges();

//            //VillaUpdateDTO villaDTO = new VillaUpdateDTO()
//            //{
//            //    Amenity = villa.Amenity,
//            //    ImageUrl = villa.ImageUrl,
//            //    Details = villa.Details,
//            //    Name = villa.Name,
//            //    Occupancy = villa.Occupancy,
//            //    Rate = villa.Rate,
//            //    Sqft = villa.Sqft,
//            //};

//            VillaUpdateDTO villaUpdate = _mapper.Map<VillaUpdateDTO>(villa);

//            // we need to apply patchDTO to vill
//            // error will be stored in model state
//            patchDTO.ApplyTo(villaUpdate, ModelState);

//            //Villa model = new Villa()
//            //{
//            //    Id = id,
//            //    Amenity = villaUpdate.Amenity,
//            //    ImageUrl = villaUpdate.ImageUrl,
//            //    Details = villaUpdate.Details,
//            //    Name = villaUpdate.Name,
//            //    Occupancy = villaUpdate.Occupancy,
//            //    Rate = villaUpdate.Rate,
//            //    Sqft = villaUpdate.Sqft,
//            //};

//            Villa model = _mapper.Map<Villa>(villaUpdate);

//            // we are updating model with same id so entity frameework is confuse here as it is already tracking same id object
//            _context.Update(model);
//            await _context.SaveChangesAsync();
//            return NoContent();

//        }

//    }
//}
