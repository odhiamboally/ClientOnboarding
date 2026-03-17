using CO.Domain.Entities;
using CO.Shared.Dtos.Client;
using System.Linq.Expressions;

namespace CO.Persistence.Extensions;

/// <summary>
/// LINQ projection extension methods for mapping entity queries directly to DTOs
/// without loading full entity graphs into memory.
///
/// Benefits:
/// - EF Core translates the Select() expression into a targeted SQL projection,
///   fetching only the columns that are actually needed.
/// - Navigation properties that are not part of the projection are never loaded,
///   eliminating unnecessary JOINs and reducing network I/O between SQL Server
///   and the application.
/// - Memory allocations are reduced because no entity change-tracker entries are
///   created for columns that are not selected.
///
/// Note on enum string values: the projections use .ToString() on enum members,
/// which EF Core translates to the enum member name (e.g. "Corporate").
/// If you need the Description-attribute value (e.g. "SME") use the in-memory
/// mapping in ClientMappings.ToClientResponse() instead.
/// </summary>
public static class ProjectionExtensions
{
    /// <summary>
    /// Projects a query to a DTO type using the supplied selector expression.
    /// This is a thin, convention-enforcing wrapper around <see cref="Queryable.Select{TSource,TResult}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Source entity type.</typeparam>
    /// <typeparam name="TDto">Target DTO type.</typeparam>
    /// <param name="query">The entity query to project.</param>
    /// <param name="selector">An EF Core-translatable expression that maps the entity to the DTO.</param>
    public static IQueryable<TDto> ProjectToDto<TEntity, TDto>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TDto>> selector)
        where TEntity : class
    {
        return query.Select(selector);
    }

    /// <summary>
    /// Projects a <see cref="StaffMember"/> query to <see cref="StaffMemberResponse"/> DTOs.
    /// Only the columns required by <see cref="StaffMemberResponse"/> are fetched from the database.
    /// </summary>
    public static IQueryable<StaffMemberResponse> ProjectToStaffMemberResponse(
        this IQueryable<StaffMember> query)
    {
        return query.Select(s => new StaffMemberResponse(
            s.Id,
            string.Concat(s.FirstName, " ", s.LastName),
            s.StaffNumber,
            s.Department));
    }

    /// <summary>
    /// Projects a <see cref="Director"/> query to <see cref="DirectorResponse"/> DTOs.
    /// Only the columns required by <see cref="DirectorResponse"/> are fetched from the database.
    /// </summary>
    public static IQueryable<DirectorResponse> ProjectToDirectorResponse(
        this IQueryable<Director> query)
    {
        return query.Select(d => new DirectorResponse(
            d.Id,
            d.FullName,
            d.RelationType.ToString(),
            d.IdentificationType.ToString(),
            d.IdentificationNumber,
            d.PhoneNumber,
            d.Email,
            d.SharePercentage,
            d.DateAdded));
    }

    /// <summary>
    /// Projects a <see cref="Client"/> query to <see cref="ClientResponse"/> DTOs.
    /// Includes an inline projection of <see cref="Director"/> children so that only
    /// the columns needed for the response are fetched; the full Director entity graph
    /// is never materialised.
    /// </summary>
    public static IQueryable<ClientResponse> ProjectToClientResponse(
        this IQueryable<Client> query)
    {
        return query.Select(c => new ClientResponse(
            // Identity & Classification
            c.Id,
            c.ClientNumber,
            c.ClientType.ToString(),
            c.SegmentType.ToString(),
            c.SubSegmentType.ToString(),
            c.Status.ToString(),
            c.OpenedOn,

            // Corporate Details
            c.CorporateDetail.CompanyName,
            c.CorporateDetail.LineOfBusiness.ToString(),
            c.CorporateDetail.LineOfBusinessMoreInfo,
            c.CorporateDetail.NatureOfBusiness,
            c.CorporateDetail.IdentificationType.ToString(),
            c.CorporateDetail.RegistrationNumber,
            c.CorporateDetail.DateOfRegistration,
            c.CorporateDetail.RegisteredAt,
            c.CorporateDetail.RegisteredOffice,
            c.CorporateDetail.BusinessStartedYear,
            c.CorporateDetail.NumberOfEmployees,
            c.CorporateDetail.Comments,
            c.CorporateDetail.Website,
            c.CorporateDetail.TINNumber,

            // Relationship Manager
            c.RelationshipManagerId,
            c.RelationshipManager != null
                ? string.Concat(c.RelationshipManager.FirstName, " ", c.RelationshipManager.LastName)
                : "—",

            // Address
            c.Address.ResidentialAddress,
            c.Address.Country,
            c.Address.Region,
            c.Address.Ward,
            c.Address.District,
            c.Address.BusinessAddress,
            c.Address.OfficeAddress,
            c.Address.MailingAddress,
            c.Address.Street,
            c.Address.ZipCode,
            c.Address.PhoneHome,
            c.Address.PhoneWork,
            c.Address.Mobile,
            c.Address.FaxNo,
            c.Address.LandMark,
            c.Address.EmailId,

            // Communication Preferences
            c.CommunicationPreference.CanSendGreetings,
            c.CommunicationPreference.CanSendAssociateSpecialOffer,
            c.CommunicationPreference.CanSendOurSpecialOffers,
            c.CommunicationPreference.StatementOnline,
            c.CommunicationPreference.MobileAlert,

            // Directors — projected inline; only needed columns are fetched
            c.Directors.Select(d => new DirectorResponse(
                d.Id,
                d.FullName,
                d.RelationType.ToString(),
                d.IdentificationType.ToString(),
                d.IdentificationNumber,
                d.PhoneNumber,
                d.Email,
                d.SharePercentage,
                d.DateAdded)).ToList()));
    }
}
