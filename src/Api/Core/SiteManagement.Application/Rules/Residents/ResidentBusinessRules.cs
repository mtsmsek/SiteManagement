using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Security.Hashing;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Domain.Constants.Residents;
using SiteManagement.Domain.Entities.Residents;

namespace SiteManagement.Application.Rules.Residents;

public class ResidentBusinessRules : BaseBusinessRules
{
    private readonly IResidentRepository _residentRepository;

    public ResidentBusinessRules(IResidentRepository residentRepository)
    {
        _residentRepository = residentRepository;
    }
    //TODO -- write checkif for insert
    public async Task<Resident> CheckIfResidentExistByIdenticalNumberWhenInsert(string identicalNumber, CancellationToken cancellationToken)
    {
        var dbResident = await _residentRepository.GetSingleAsync(predicate: resident => resident.IdenticalNumber == identicalNumber);

        //TODO -- Remove magic string
        if (dbResident is not null)
            throw new BusinessException(ResidentMessages.RuleMessages.ResidentAlreadyExists);


        return dbResident!;
    }
    public async Task<Resident> CheckIfResidentExistByEmailWhenInsert(string email, CancellationToken cancellationToken)
    {
        var dbResident = await _residentRepository.GetSingleAsync(predicate: resident => resident.Email == email);

        //TODO -- Remove magic string
        if (dbResident is not null)
            throw new BusinessException(ResidentMessages.RuleMessages.EmailCannotBeDuplicated);


        return dbResident!;
    }
    public async Task<Resident> CheckIfResidentExistByIdenticalNumberWhenLogin(string identicalNumber, CancellationToken cancellationToken)
    {
        var dbResident = await _residentRepository.GetSingleAsync(predicate: resident => resident.IdenticalNumber == identicalNumber);

        //TODO -- Remove magic string
        if(dbResident is null)
            throw new BusinessException(ResidentMessages.RuleMessages.ResidentCannotBeFound);

            
        return dbResident!;
    }
    public async Task<Resident> CheckIfResidentExistByEmailWhenLogin(string email, CancellationToken cancellationToken)
    {
        var dbResident = await _residentRepository.GetSingleAsync(predicate: resident => resident.Email == email,cancellationToken: cancellationToken);


        if (dbResident is null)
            throw new BusinessException(ResidentMessages.RuleMessages.ResidentCannotBeFound);


        return dbResident!;
    }
    public async Task<Resident> CheckIfResidentExistById(Guid id, CancellationToken cancellationToken)
    {
        var dbResident = await _residentRepository.GetByIdAsync(id,cancellationToken: cancellationToken);

        if (dbResident is null)
            throw new BusinessException(ResidentMessages.RuleMessages.ResidentCannotBeFound);


        return dbResident!;
    }
    public void CheckIfPasswordIsTrue(string password, byte[] passwordHash, byte[] passwordSalt)
    {
       var result = HashingHelper.VerifyPasswordHash(password, passwordHash, passwordSalt);


        if (!result)
            throw new BusinessException(ResidentMessages.RuleMessages.WrongPassword);
        
        return;
    }
    public void EmailCannotBeSameWithOldEmail(string newEmail, string oldEmail)
    {
        if (newEmail == oldEmail)
            throw new BusinessException(ResidentMessages.RuleMessages.NewEmailCannotBeSameWithTheOldEmail);

    }
}
