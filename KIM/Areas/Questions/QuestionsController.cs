using KIM.BL.Areas.Questions.Commands;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Areas.Questions.Queries;
using KIM.BL.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KIM.Api.Areas.Questions;

[ApiController]
[Route("api/questions")]
public class QuestionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("rating")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Rate(
        [FromBody] RateQuestionCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionDto>>>> Get([FromQuery] GetQuestionsQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<QuestionLookupDto>>>> GetLookup([FromQuery] GetQuestionsLookupQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetQuestionByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Create([FromBody] CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionDto>>> Update(Guid id, [FromBody] UpdateQuestionCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteQuestionCommand { Id = id }, cancellationToken);
        return Ok(response);
    }
}