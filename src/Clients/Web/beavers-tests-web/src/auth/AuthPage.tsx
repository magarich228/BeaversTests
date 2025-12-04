import { observer } from "mobx-react-lite";
import { AppHeader } from "../shared/components/Header";

export const AuthPage: React.FC = observer(() => {
    return (
        <div style={{width: '100vw', height: '100vh', display: 'grid', gridTemplateColumns: 'auto 620px auto', gridTemplateRows: 'auto 1fr', minWidth: '620px'}}>
            <div style={{gridColumn: '1/4', border: '1px solid black'}}>
                <AppHeader/>
            </div>
            <div style={{gridColumn: '2/3', border: '1px solid black'}}>
            </div>
        </div>
    );
});