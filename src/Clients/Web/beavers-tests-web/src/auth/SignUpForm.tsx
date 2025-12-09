import { useStore } from "../shared/stores";
import { useState } from "react";

export const SignUpForm: React.FC = () => {
    const [authData, setAuthData] = useState(
    { 
        email: '', 
        password: '',
        displayName: ''
    });
        
    const { authStore } = useStore();
    
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const { name, value } = e.target;
        setAuthData(prev => ({ ...prev, [name]: value}));
        authStore.clearError();
    };

    const handleSubmit = async (e: React.FormEvent): Promise<void> => {
        e.preventDefault();
        await authStore.register(authData);
    };

    return (
        <div style={{border: '1px solid black', width: '310px', justifyContent: 'center', height: 'fit-content'}}>
            <div>
                <h2 style={{textAlign: 'center'}}>Sing Up</h2>
            </div>
            <div>
                <form
                    onSubmit={handleSubmit} 
                    style={{display: 'flex', flexDirection: 'column', padding: '18px', gap: '16px', alignItems: 'stretch'}}>
                    <div>
                        <div>
                            <label htmlFor="email">Email</label>
                        </div>
                        <div>
                            <input id="email" name="email" type='email' placeholder='Email' onChange={handleChange} autoComplete="on" style={{width: '100%', boxSizing: 'border-box'}}/>
                        </div>
                    </div>
                    <div>
                        <div>
                            <label htmlFor="password">Password</label>
                        </div>
                        <div>
                            <input id="password" name="password" type='password' placeholder="Password" onChange={handleChange} autoComplete="on" style={{width: '100%', boxSizing: 'border-box'}}/>
                        </div>
                    </div>
                    <div>
                        <div>
                            <label htmlFor="displayName">Username</label>
                        </div>
                        <div>
                            <input id="displayName" name="displayName" type='displayName' placeholder="Display name" onChange={handleChange} autoComplete="on" style={{width: '100%', boxSizing: 'border-box'}}/>
                        </div>
                    </div>
                    <div style={{justifyItems: 'center', marginTop: '12px'}}>
                        <button type="submit" style={{display: 'block'}}>Sign Up</button>
                    </div>
                </form>
            </div>
        </div>
    );
};