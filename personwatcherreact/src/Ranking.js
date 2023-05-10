import React, {Component} from 'react';
import { Table } from 'react-bootstrap';
import moment from 'moment';
import {If, Then, ElseIf, Else} from 'react-if-elseif-else-render';

export class Ranking extends Component {
    constructor(props) {
        super(props);
        this.state={persons:[], AppModalShow:false};
    }

    refreshList(){
        fetch(process.env.REACT_APP_API+'Person/Rank')
        .then(response=>response.json())
        .then(data=>{
            this.setState({persons:data});
        })
    }

    componentDidMount() {
        this.refreshList();
    }

    render() {
        const {persons, personid}=this.state;
        const currentDate = moment(new Date());
        let addModalClose=()=>this.setState({addModalShow:false});
        let editModalClose=()=>this.setState({editModalShow:false});
           return (
            <div >
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Next Start</th>
                            <th>Pluses</th>
                            <th>Minuses</th>
                            <th>Predictability</th>
                            <th>Sign Matches</th>
                            <th>Young</th>
                            <th>Experienced</th>
                            <th>Next Action</th>
                            <th>Impulsivity</th>
                        </tr>
                    </thead>
                    <tbody>
                        {persons.map(person=>
                            <tr key={person.personId}>
                                <td>{person.name}</td>
                                <If condition={moment(person.nextStart).diff(currentDate, 'minutes') < -180}>
                                    <Then>
                                        <td style={{backgroundColor:"coral"}}>{moment(person.nextStart).calendar()}</td>
                                    </Then>
                                    <ElseIf condition={moment(person.nextStart).diff(currentDate, 'minutes') < 15}>
                                        <td style={{backgroundColor:"turquoise"}}>{moment(person.nextStart).calendar()}</td>
                                    </ElseIf>
                                    <Else>
                                        <td>{moment(person.nextStart).calendar()}</td>
                                    </Else>
                                </If>
                                <td>{person.sunPos}</td>
                                <td>{person.moonPos}</td>
                                <td>{person.eventPredictability}</td>
                                <td>{person.venusPos}</td>
                                <td>{person.marsPos}</td>
                                <td>{person.saturnPos}</td>
                                <td>{moment(person.nextStart).add(person.jupiterPos, 'minute').format('HH:mm')}</td>
                                <td>{person.neptunePos}</td>
                            </tr>)}
                    </tbody>
                </Table>
            </div>
        )
    }
}