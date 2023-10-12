function AppCreate() {
    return (
        <>
            <h1>Create new app</h1>
            <form>
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