﻿using MediatR;

namespace BeaversTests.Common.CQRS.Queries;

public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{ }