export const login = async (email: string, password: string) => {
    try {
        const res = await fetch('', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        })
        return res.json();
    } catch (error) {
        
    }
};
