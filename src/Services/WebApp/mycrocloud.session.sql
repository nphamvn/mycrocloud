SELECT
	war.route_id RouteId,
	war.web_application_id WebAppId,
	war."name" Name,
    war.description Description,
	"path" MatchPath,
    COALESCE(JSON_AGG(rmmb.method) FILTER (WHERE rmmb.method IS NOT NULL), '[]') MatchMethods,
    war.match_order MatchOrder,
	war.created_date CreatedDate,
    war.updated_date UpdatedDate
FROM
	web_application_route war
INNER JOIN
	web_application wa ON wa.web_application_id = war.web_application_id
    LEFT JOIN web_app_route_match_method_bind rmmb ON rmmb.route_id = war.route_id
--WHERE 
--	wa.user_id = @user_id and wa."name" = @web_app_name
GROUP BY
    war.route_id