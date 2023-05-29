﻿namespace Ocelot.Errors
{
    public enum OcelotErrorCode
    {
        UnauthenticatedError = 0,
        UnknownError = 1,
        DownstreampathTemplateAlreadyUsedError = 2,
        UnableToFindDownstreamRouteError = 3,
        CannotAddDataError = 4,
        CannotFindDataError = 5,
        UnableToCompleteRequestError = 6,
        UnableToCreateAuthenticationHandlerError = 7,
        UnsupportedAuthenticationProviderError = 8,
        CannotFindClaimError = 9,
        ParsingConfigurationHeaderError = 10,
        NoInstructionsError = 11,
        InstructionNotForClaimsError = 12,
        UnauthorizedError = 13,
        ClaimValueNotAuthorizedError = 14,
        ScopeNotAuthorizedError = 15,
        UserDoesNotHaveClaimError = 16,
        DownstreamPathTemplateContainsSchemeError = 17,
        DownstreamPathNullOrEmptyError = 18,
        DownstreamSchemeNullOrEmptyError = 19,
        DownstreamHostNullOrEmptyError = 20,
        ServicesAreNullError = 21,
        ServicesAreEmptyError = 22,
        UnableToFindServiceDiscoveryProviderError = 23,
        UnableToFindLoadBalancerError = 24,
        RequestTimedOutError = 25,
        UnableToFindQoSProviderError = 26,
        UnmappableRequestError = 27,
        RateLimitOptionsError = 28,
        PathTemplateDoesntStartWithForwardSlash = 29,
        FileValidationFailedError = 30,
        UnableToFindDelegatingHandlerProviderError = 31,
        CouldNotFindPlaceholderError = 32,
        CouldNotFindAggregatorError = 33,
        CannotAddPlaceholderError = 34,
        CannotRemovePlaceholderError = 35,
        QuotaExceededError = 36,
        RequestCanceled = 37,
        ConnectionToDownstreamServiceError = 38,
        CouldNotFindLoadBalancerCreator = 39,
        ErrorInvokingLoadBalancerCreator = 40,
        UnableToFindDownstreamWebApplicationError = 41,
        WebApplicationIsBlockedError = 42,
        WebApplicationIsNotEnabledError = 43
    }
}
