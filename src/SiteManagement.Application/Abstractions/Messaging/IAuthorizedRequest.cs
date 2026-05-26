namespace SiteManagement.Application.Abstractions.Messaging;

/// <summary>
/// Base marker for the authorization requirement every MediatR request must
/// declare. <c>AuthorizationBehavior</c> reads it to enforce access from one
/// place, and an architecture test asserts that every command/query implements
/// exactly one concrete marker below — so authorizing an endpoint is a
/// conscious, un-forgettable decision, never a controller attribute someone can
/// omit unnoticed.
/// </summary>
public interface IAuthorizedRequest;

/// <summary>Restricts the request to callers in the Admin role.</summary>
public interface IAdminRequest : IAuthorizedRequest;

/// <summary>
/// Restricts the request to an authenticated resident (Resident role with a
/// linked resident id). The behavior proves "a resident"; the handler still
/// proves "<em>this</em> resident's resource" (ownership needs data the
/// pipeline doesn't have).
/// </summary>
public interface IResidentRequest : IAuthorizedRequest;

/// <summary>
/// Marks the request as intentionally open (e.g. login, token refresh). Being
/// explicit keeps "no authorization" a declared choice the architecture test
/// can see rather than an oversight.
/// </summary>
public interface IPublicRequest : IAuthorizedRequest;
