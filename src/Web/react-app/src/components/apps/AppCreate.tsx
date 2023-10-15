import AppConfig from "../../constants/AppConfig"
import { useAuth } from "../../hooks/useAuth";

function AppCreate() {
    const { user } = useAuth()!;
    const onSubmit = async () => {
        try {
            await fetch(`${AppConfig}/apps/create`, {
                method: 'POST',
                headers: {
                    Authorzation: `Bearer ${user?.accessToken}`
                }
            })
        } catch (error) {
            console.log(error);
        }
    }
    return (
        <>
            <h1>Create new app</h1>
            <form onSubmit={onSubmit}>
                <div>
                    <label htmlFor="name">Name</label>
                    <input type="text" name="name"></input>
                </div>
                <button type="submit">Create</button>
            </form>
        </>
    )
}
export default AppCreate