using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmeController : ControllerBase
    {
        private AppDbContext _context;
        private IMapper _mapper;

        public FilmeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpPost]
        public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
        {
            // Filme filme = new Filme
            // {
            //     Titulo = filmeDto.Titulo,
            //     Genero = filmeDto.Genero,
            //     Duracao = filmeDto.Duracao,
            //     Diretor = filmeDto.Diretor
            // };
            // ---- Implementando AutoMapper: ----
            // Instancia do AutoMapper, convertendo para um Filme, a partir de CreateFilmeDto
            Filme filme = _mapper.Map<Filme>(filmeDto);

            _context.Filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFilmePorId), new { Id = filme.Id }, filme);
        }

        [HttpGet]
        public IActionResult RecuperaFilmes([FromQuery] int? classificacaoEtaria = null)
        {
            List<Filme> filmes;
            if(classificacaoEtaria == null)
            {
                filmes = _context.Filmes.ToList();
            }
            else
            {
                filmes = _context
                    .Filmes.Where(filme => filme.ClassificacaoEtaria <= classificacaoEtaria).ToList();
            }
            
            if(filmes != null)
            {
                List<ReadFilmeDto> readtDto = _mapper.Map<List<ReadFilmeDto>>(filmes);
                return Ok(readtDto);
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult RecuperaFilmePorId(int id)
        {
            // foreach(Filme filme in filmes) {
            //     if (filme.Id == id)
            //         return  Ok(filme);
            // }
            // return NotFound();

            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if(filme != null)
            {
                // ReadFilmeDto filmeDto = new ReadFilmeDto
                // {
                //     Titulo = filme.Titulo,
                //     Diretor = filme.Diretor,
                //     Duracao = filme.Duracao,
                //     Genero = filme.Genero,
                //     Id = filme.Id,
                //     HoraDaConsulta = DateTime.Now
                // };
                // ---- Implementando AutoMapper: ----
                // Instancia do AutoMapper, convertendo de Filme, para de ReadFilmeDto
                ReadFilmeDto filmeDto = _mapper.Map<ReadFilmeDto>(filme);    

                return Ok(filmeDto);
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if(filme == null)
                return NotFound();
            
            // filme.Titulo = filmeDto.Titulo;
            // filme.Genero = filmeDto.Genero;
            // filme.Duracao = filmeDto.Duracao;
            // filme.Diretor = filmeDto.Diretor;
            // ---- Implementando AutoMapper: ----
            // Sobrescrevendo as informações do filme com as informações de filmeDto
            _mapper.Map(filmeDto, filme);

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletaFilme(int id)
        {
            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null)
                return NotFound();
            
            _context.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
}