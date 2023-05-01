import React from 'react';
import { AsyncTypeahead } from 'react-bootstrap-typeahead';

//import options from './data';

import 'react-bootstrap-typeahead/css/Typeahead.css';
const SEARCH_URI = process.env.REACT_APP_API+'Place';

export class PlaceTypeahead extends React.Component {
  state = {
    isLoading: false,
    selected: [],
    options: [],
    value: ''
  };

  _handleSearch = query => {
    this.setState({ isLoading: true });
    this.makeAndHandleRequest(query).then(({ opts }) => {
      this.setState(
        {
          isLoading: false,
          options: opts
        }
      );
    });
  };

   makeAndHandleRequest(query, page = 1) {
    return fetch(`${SEARCH_URI}?searchStr=${query}`)
      .then(resp => resp.json())
      .then(data => {
        const total_count=data.length;
        const opts = data.map(i => ({
          placeId: i.placeId,
          placeName: i.placename + "(" + i.latitude.substring(0, 6) + ", " + i.longitude.substring(0, 7) + ")"
        }));
        return { opts, total_count };
      });
  }

  render() {
    return (
      <AsyncTypeahead
        {...this.state}
        id="placeId"
        labelKey="placeName"
        clearButton
        onSearch={this._handleSearch}
        onChange={selected => {this.setState({selected});
          this.props.onChangeValue({selected})}}
        options={this.state.options}
        placeholder="Choose a place..."
      />
    );
  }
}

