import { useAuth0 } from "@auth0/auth0-react";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function AdminGuardPage({ children }: { children: React.ReactNode }) {
    const navigate = useNavigate();
    const { getIdTokenClaims, getAccessTokenSilently } = useAuth0();
    const [isLoading, setIsLoading] = useState(true);
    const [isAdmin, setIsAdmin] = useState(false);

    useEffect(() => {
        const getRole = async () => {
            console.log('fetching role')
            setIsLoading(true);
            try {
                const accessToken = await getAccessTokenSilently();
                console.log('accessToken:', accessToken);
                const claims = await getIdTokenClaims();
                
                if (!claims) {
                    return;
                }
                console.log('claims:', claims)
                const idToken = claims.__raw;
                console.log('idToken:', idToken);
                const permissions = claims["permissions"];
                console.log('permissions:', permissions);
                setIsAdmin(true)
            } catch (error) {

            } finally {
                setIsLoading(false);
            }
        };

        getRole();
    }, []);

    if (isLoading) {
        return null;
    }

    if (!isAdmin) {
        //navigate('/');
        return null;
    }

    return (<>{children}</>)
}