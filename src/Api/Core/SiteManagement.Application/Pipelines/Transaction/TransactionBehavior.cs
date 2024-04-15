using MediatR;
using System.Transactions;

namespace SiteManagement.Application.Pipelines.Transaction
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ITransactionalRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
           using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            TResponse response;

            try
            {
                response = await next();
                scope.Complete();
            }
            catch (Exception)
            {
                //todo create new exception
                scope.Dispose();
                throw;
            }

            return response;
        }
    }
}
