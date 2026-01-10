using GTMS.Application.Features.Thesis.ThesisProjects.Dtos;
using MediatR;

namespace GTMS.Application.Features.Thesis.ThesisProjects.Queries.GetMyThesisProjects;

public record GetMyThesisProjectsQuery : IRequest<List<ThesisProjectDto>>;
