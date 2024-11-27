using DealW.Application.Services;
using DealW.Contracts;
using DealW.Domain.Models;
using DealW.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DealW.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizzesService _quizzesService;

    public QuizzesController(IQuizzesService quizzesService)
    {
        _quizzesService = quizzesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuizzesResponse>>> GetQuizzes()
    {
        var quizzes = await _quizzesService.GetAllQuizzes();

        var response = quizzes.Select(q => new QuizzesResponse(q.Id, q.Title, q.Questions));
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateQuiz([FromBody] QuizzesRequest request)
    {
        var (quiz, error) = Quiz.Create(
            Guid.NewGuid(),
            request.title,
            request.questions);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        
        var quizId = await _quizzesService.CreateQuiz(quiz);
        
        return Ok(quizId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateQuizzes(Guid id, [FromBody] QuizzesRequest request)
    {
        var quizId = await _quizzesService.UpdateQuiz(id, request.title, request.questions);
        
        return Ok(quizId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteQuiz(Guid id)
    {
        return Ok(await _quizzesService.DeleteQuiz(id));
    }
}