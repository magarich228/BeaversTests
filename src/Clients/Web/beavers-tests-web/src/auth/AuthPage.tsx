import { observer } from "mobx-react-lite";
import { AppHeader } from "../shared/components/Header";

export const AuthPage: React.FC = observer(() => {
    return (
        <div style={{width: '100vw', height: '100vh', display: 'grid', gridTemplateColumns: 'auto 620px auto', gridTemplateRows: 'auto 1fr', minWidth: '620px'}}>
            <div style={{gridColumn: '1/4', border: '1px solid black'}}>
                <AppHeader/>
            </div>
            <div style={{gridColumn: '2/3', border: '1px solid black', display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
                <div style={{height: '100px'}}/>
                <div style={{border: '1px solid black', width: '310px', justifyContent: 'center', height: 'fit-content'}}>
                    <div>
                        <h2 style={{textAlign: 'center'}}>Sing In</h2>
                    </div>
                    <div>
                        <form style={{display: 'flex', flexDirection: 'column', padding: '18px', gap: '16px', alignItems: 'stretch'}}>
                            <div>
                                <div>
                                    <label htmlFor="email">Email</label>
                                </div>
                                <div>
                                    <input id="email" type='email' placeholder='Email' style={{width: '100%', boxSizing: 'border-box'}}/>
                                </div>
                            </div>
                            <div>
                                <div>
                                    <label htmlFor="password">Password</label>
                                </div>
                                <div>
                                    <input id="password" type='password' placeholder="Password" style={{width: '100%', boxSizing: 'border-box'}}/>
                                </div>
                            </div>
                            <div style={{justifyItems: 'center', marginTop: '12px'}}>
                                <button type="submit" style={{display: 'block'}}>SignIn</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
});