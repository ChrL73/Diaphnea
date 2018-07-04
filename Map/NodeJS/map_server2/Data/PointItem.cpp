/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

#include "PointItem.h"
#include "PointElement.h"
#include "PointLook.h"

namespace map_server
{
    PointItem::PointItem(const PointElement *pointElement, const NameTranslation *name, const PointLook *pointLook) :
        MapItem(pointElement->getItemId(), pointElement->getElementId(), pointElement->getImportance(), pointElement->getX(), pointElement->getX(),
        pointElement->getY(), pointElement->getY(), 3 * pointLook->getLookId(),
        pointLook->getTextColor(), pointLook->getTextOpacity(), pointLook->getTextSize(), pointElement->getFramingLevel(), name),
        _pointLookId(3 * pointLook->getLookId() + 1), _pointZIndex(pointLook->getPointZIndex()),
        _pointColor(pointLook->getPointColor()), _pointOpacity(pointLook->getPointOpacity()),
        _pointSize(pointLook->getPointSize())
    {
    }
}
