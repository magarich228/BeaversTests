import { observer } from "mobx-react-lite";
import { AppHeader } from "../shared/components/Header";
import { useStore } from "../shared/stores";
import { NavigateFunction, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export const AuthPage: React.FC = observer(() => {
    const [authData, setAuthData] = useState(
        { 
            email: '', 
            password: '' 
        });
    
    const { authStore } = useStore();
    const navigate: NavigateFunction = useNavigate();

    useEffect(() => {
        if (authStore.isAuthenticated)
            navigate('/test');
    }, [authStore.isAuthenticated, navigate]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const { name, value } = e.target;
        setAuthData(prev => ({ ...prev, [name]: value}));
        authStore.clearError();
    };

    const handleSubmit = async (e: React.FormEvent): Promise<void> => {
        e.preventDefault();
        await authStore.login(authData);
    };

    // TODO: Выделить компоненты форм, реализовать кастомные инпуты
    // TODO: На формах реализовать парсинг и вывод ошибок. Возможно в зависимости от провайдера аутентификации 
    // (возвращать с сервера или подумать над контрактом ошибок аутентификации, авторизации для всех провайдеров)
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